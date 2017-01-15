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
        public int DataTransferPort { get; }

        public ConnectionComponent(string connectionManagerIpAddress, int connectionManagerListeningPort, int dataTransferPort) {
            _ipAddress = IPAddress.Parse(connectionManagerIpAddress);
            _connectionManagerListeningPort = connectionManagerListeningPort;
            DataTransferPort = dataTransferPort;

            Initialize();
        }

        private void Initialize() {
            _connectionManagerTcpListener = CreateTcpListener(_ipAddress, DataTransferPort);
            ListenForConnectRequest(_connectionManagerTcpListener);
            EstabilishConnection();
        }

        private TcpListener CreateTcpListener(IPAddress ipAddress, int port) {
            TcpListener tcpListener = null;
            try {
                tcpListener = new TcpListener(ipAddress, port);
            }
            catch (Exception e) {
                OnUpdateState($"Can't connect to port {DataTransferPort}!");
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
                var cableCloudMessage = (CableCloudMessage) ReceiveObject();
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
            var bytesToSend = BinarySerializer.Serialize(DataTransferPort);
            udpClient.Send(bytesToSend, bytesToSend.Length, ipEndPoint);
        }


        public void SendObject(object objectToSend) {
            var networkStream = _connectionObjectTcpClient.GetStream();
            BinarySerializer.SerializeToStream(objectToSend, networkStream);
        }
    }
}
