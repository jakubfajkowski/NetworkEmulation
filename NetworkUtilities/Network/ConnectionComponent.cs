using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NetworkUtilities.Log;
using NetworkUtilities.Utilities;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkUtilities.Network {
    public class ConnectionComponent : LogObject, IDisposable {
        public delegate void ObjectHandler(object sender, object receivedObject);
        public delegate void ConnectionHandler(object sender, EventArgs args);

        private readonly int _connectionManagerListeningPort;
        private readonly ConnectionManagerType _connectionManagerType;
        private readonly ConnectionRequestMessage _handshakeMessage;
        private readonly IPAddress _ipAddress;

        private TcpListener _tcpListener;
        private TcpClient _tcpClient;
        private UdpClient _udpClient;

        public ConnectionComponent(NetworkAddress networkAddress, string connectionManagerIpAddress,
            int connectionManagerListeningPort, ConnectionManagerType connectionManagerType) {
            _ipAddress = IPAddress.Parse(connectionManagerIpAddress);
            _connectionManagerListeningPort = connectionManagerListeningPort;
            _connectionManagerType = connectionManagerType;
            _handshakeMessage = new ConnectionRequestMessage(networkAddress, PortRandomizer.RandomFreePort());
            _connectionManagerType = connectionManagerType;
        }

        public bool Online { get; private set; }
        public event ObjectHandler ObjectReceived;
        public event ConnectionHandler ConnectionEstablished;

        public void Initialize() {
            _tcpListener = CreateTcpListener(_ipAddress, _handshakeMessage.SocketPort);
            ListenForConnectRequest(_tcpListener);
            EstabilishConnection();
        }

        private TcpListener CreateTcpListener(IPAddress ipAddress, int port) {
            TcpListener tcpListener = null;
            try {
                tcpListener = new TcpListener(ipAddress, port);
            }
            catch (Exception e) {
                OnUpdateState($"[CONNECTION_REQUEST] {_connectionManagerType} Rejected");
            }

            return tcpListener;
        }

        private void EstabilishConnection() {
            var ipEndPoint = new IPEndPoint(IPAddress.Loopback, _connectionManagerListeningPort);
            SendHandshakeMessage(ipEndPoint);
        }

        private void SendHandshakeMessage(IPEndPoint ipEndPoint) {
            _udpClient = new UdpClient();
            var bytesToSend = BinarySerializer.Serialize(_handshakeMessage);
            _udpClient.Send(bytesToSend, bytesToSend.Length, ipEndPoint);
        }

        private void ListenForConnectRequest(TcpListener tcpListener) {
            tcpListener.Start();
            Task.Run(async () => {
                OnUpdateState($"[CONNECTION_REQUEST] {_connectionManagerType} Waiting");
                _tcpClient = await tcpListener.AcceptTcpClientAsync();
                Online = true;

                OnUpdateState($"[CONNECTION_REQUEST] {_connectionManagerType} Accepted");
                OnConnectionEstablished();
                tcpListener.Stop();
                ListenForMessages();
            });
        }

        private void ListenForMessages() {
            while (Online) {
                try {
                    var receivedObject = Receive();
                    OnObjectReceived(receivedObject);
                }
                catch (IOException) {
                    OnUpdateState($"[CLOSED] {_connectionManagerType}");
                }
                catch (SerializationException) {
                    OnUpdateState($"[READING_ERROR] {_connectionManagerType}");
                }
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

        public void Dispose() {
            _udpClient?.Dispose();
            _tcpClient?.Dispose();
            Online = false;
        }
    }
}