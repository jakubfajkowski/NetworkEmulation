using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities.Log;
using NetworkUtilities.Utilities;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkUtilities.Network {
    public class ConnectionComponent : LogObject {
        private readonly int _connectionManagerListeningPort;
        private readonly NetworkAddressSocketPortPair _handshakeMessage;
        private readonly IPAddress _ipAddress;

        private TcpListener _connectionManagerTcpListener;
        private TcpClient _tcpClient;

        public ConnectionComponent(NetworkAddress networkAddress, string connectionManagerIpAddress,
            int connectionManagerListeningPort) {
            _ipAddress = IPAddress.Parse(connectionManagerIpAddress);
            _connectionManagerListeningPort = connectionManagerListeningPort;
            _handshakeMessage = new NetworkAddressSocketPortPair(networkAddress, PortRandomizer.RandomFreePort());
        }

        public bool Online { get; private set; }
        public event ObjectHandler ObjectReceived;
        public event ConnectionHandler ConnectionEstablished;

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
                OnUpdateState($"Sent connection request - rejected");
            }

            return tcpListener;
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

        private void ListenForConnectRequest(TcpListener tcpListener) {
            tcpListener.Start();
            Task.Run(async () => {
                OnUpdateState($"Sent connection request - waiting for response");
                _tcpClient = await tcpListener.AcceptTcpClientAsync();
                Online = true;

                OnUpdateState($"Sent connection request - accepted");
                OnConnectionEstablished();
                ListenForMessages();
            });
        }

        private void ListenForMessages() {
            while (Online) {
                var receivedObject = Receive();
                OnObjectReceived(receivedObject);
            }
        }

        private object Receive() {
            var networkStream = _tcpClient.GetStream();
            return BinarySerializer.DeserializeFromStream(networkStream);
        }

        protected virtual void OnObjectReceived(object receivedObject) {
            ObjectReceived?.Invoke(this, receivedObject);
        }

        private void OnConnectionEstablished() {
            ConnectionEstablished?.Invoke(this, new EventArgs());
        }

        public void Send(object objectToSend) {
            var networkStream = _tcpClient.GetStream();
            BinarySerializer.SerializeToStream(objectToSend, networkStream);
        }

        public delegate void ObjectHandler(object sender, object receivedObject);
    }

    public delegate void ConnectionHandler(object sender, EventArgs args);
}