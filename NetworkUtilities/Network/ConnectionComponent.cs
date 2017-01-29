using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities.Log;
using NetworkUtilities.Serialization;

namespace NetworkUtilities.Network {
    internal class ConnectionComponent : LogObject {
        internal delegate void ObjectHandler(object sender, object receivedObject);
        public event ObjectHandler ObjectReceived;

        private TcpListener _connectionManagerTcpListener;
        private TcpClient _connectionObjectTcpClient;
        private readonly int _connectionManagerListeningPort;
        private readonly IPAddress _ipAddress;

        public bool Online { get; private set; }
        private readonly NetworkAddressSocketPortPair _handshakeMessage;

        public ConnectionComponent(NetworkAddress networkAddress, string connectionManagerIpAddress, int connectionManagerListeningPort) {
            _ipAddress = IPAddress.Parse(connectionManagerIpAddress);
            _connectionManagerListeningPort = connectionManagerListeningPort;
            _handshakeMessage = new NetworkAddressSocketPortPair(networkAddress, PortRandomizer.RandomFreePort());
        }

        public void Initialize() {
            _connectionManagerTcpListener = CreateTcpListener(_ipAddress, _handshakeMessage.SocketPort);
            ListenForConnectRequest(_connectionManagerTcpListener);
            EstabilishConnection();
        }

        private TcpListener CreateTcpListener(IPAddress ipAddress, int port) {
            TcpListener tcpListener = null;
            try {
                tcpListener = new TcpListener(ipAddress, port);
            }
            catch (Exception e) {
                OnUpdateState($"Can't connect to port {_handshakeMessage}!");
            }

            return tcpListener;
        }

        private void ListenForConnectRequest(TcpListener tcpListener) {
            tcpListener.Start();
            Task.Run(async () => {
                _connectionObjectTcpClient = await tcpListener.AcceptTcpClientAsync();
                Online = true;
                ListenForMessages();
            });
        }

        private void ListenForMessages() {
            while (Online) {
                var cableCloudMessage = ReceiveObject();
                OnObjectReceived(cableCloudMessage);
            }
        }
        protected virtual void OnObjectReceived(object receivedObject) {
            ObjectReceived?.Invoke(this, receivedObject);
        }

        private object ReceiveObject() {
            var networkStream = _connectionObjectTcpClient.GetStream();
            return BinarySerializer.DeserializeFromStream(networkStream);
        }

        private void EstabilishConnection() {
            var ipEndPoint = new IPEndPoint(IPAddress.Loopback, _connectionManagerListeningPort);
            SendHandshakeMessage(ipEndPoint);
        }

        private void SendHandshakeMessage(IPEndPoint ipEndPoint) {
            var udpClient = new UdpClient();
            var bytesToSend = BinarySerializer.Serialize(_handshakeMessage);
            udpClient.Send(bytesToSend, bytesToSend.Length, ipEndPoint);
        }


        public void SendObject(object objectToSend) {
            var networkStream = _connectionObjectTcpClient.GetStream();
            BinarySerializer.SerializeToStream(objectToSend, networkStream);
        }
    }
}
