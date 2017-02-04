using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NetworkUtilities.Log;
using NetworkUtilities.Utilities;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkUtilities.Network {
    public abstract class ConnectionManager : LogObject, IDisposable {
        private readonly Dictionary<NetworkAddress, TcpClient> _nodesTcpClients;
        private UdpClient _udpClient;

        public ConnectionManagerType ConnectionManagerType { get; }

        protected ConnectionManager(int listeningPort, ConnectionManagerType connectionManagerType) {
            _nodesTcpClients = new Dictionary<NetworkAddress, TcpClient>();
            ListeningPort = listeningPort;
            ConnectionManagerType = connectionManagerType;
        }

        public int ListeningPort { get; }

        public bool Online { get; private set; }

        public void StartListening() {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, ListeningPort);
            _udpClient = new UdpClient(ipEndPoint);

            ListenForConnectionRequests();
            OnUpdateState($"[RISE_AND_SHINE] {ConnectionManagerType}");
        }

        private void ListenForConnectionRequests() {
            Task.Run(async () => {
                using (_udpClient) {
                    Online = true;
                    while (Online) {
                        var receivedData = await _udpClient.ReceiveAsync();
                        ConnectToNode(
                            (ConnectionRequestMessage) BinarySerializer.Deserialize(receivedData.Buffer));
                    }
                }
            });
        }

        private void ConnectToNode(ConnectionRequestMessage connectionRequestMessage) {
            var nodeTcpClient = new TcpClient();

            var port = connectionRequestMessage.SocketPort;
            var clientNetworkAddress = connectionRequestMessage.NetworkAddress;

            try {
                EstablishConnection(nodeTcpClient, port, clientNetworkAddress);
                OnUpdateState($"[ACCEPTED] {clientNetworkAddress}");
                Listen(nodeTcpClient, clientNetworkAddress).Start();
            }
            catch (SocketException e) {
                DeleteConnection(clientNetworkAddress);
                OnUpdateState($"[FAILED] {clientNetworkAddress}");
                OnUpdateState(e.Message);
            }
        }

        private void EstablishConnection(TcpClient nodeTcpClient, int port, NetworkAddress networkAddress) {
            nodeTcpClient.Connect(IPAddress.Loopback, port);
            AddConnection(networkAddress, nodeTcpClient);
        }

        public void AddConnection(NetworkAddress networkAddress, TcpClient nodeTcpClient) {
            _nodesTcpClients.Add(networkAddress, nodeTcpClient);
        }

        public void DeleteConnection(NetworkAddress networkAddress) {
            _nodesTcpClients.Remove(networkAddress);
        }

        private Task Listen(TcpClient nodeTcpClient, NetworkAddress clientNetworkAddress) {
            return new Task(() => {
                try {
                    while (Online) {
                        var receivedObject = Receive(nodeTcpClient.GetStream());
                        HandleReceivedObject(receivedObject, clientNetworkAddress);
                    }
                }
                catch (IOException) {
                    OnUpdateState($"[CLOSED] {clientNetworkAddress}");
                    DeleteConnection(clientNetworkAddress);
                }
                catch (SerializationException) {
                    OnUpdateState($"[CLOSED] {clientNetworkAddress}");
                    DeleteConnection(clientNetworkAddress);
                }
            });
        }

        private object Receive(Stream networkStream) {
            return BinarySerializer.DeserializeFromStream(networkStream);
        }

        protected abstract void HandleReceivedObject(object receivedObject, NetworkAddress inputNetworkAddress);

        protected void Send(object objectToSend, NetworkAddress outputNetworkAddress) {
            try {
                var tcpClient = _nodesTcpClients[outputNetworkAddress];
                var networkStream = tcpClient.GetStream();

                BinarySerializer.SerializeToStream(objectToSend, networkStream);
            }
            catch (KeyNotFoundException) {
                OnUpdateState($"[ADDRESS_NOT_FOUND] {outputNetworkAddress}");
            }
        }

        public void Dispose() {
            OnUpdateState($"[HELLO_DARKNESS_MY_OLD_FRIEND] {ConnectionManagerType}");
            Online = false;
            _udpClient.Close();
            Online = false;
        }

        public bool AreConnected(List<NetworkAddress> networkAddresses) {
            foreach (var address in networkAddresses) if (!IsConnected(address)) return false;
            return true;
        }

        public bool IsConnected(NetworkAddress networkAddress) {
            return _nodesTcpClients.ContainsKey(networkAddress);
        }
    }
}