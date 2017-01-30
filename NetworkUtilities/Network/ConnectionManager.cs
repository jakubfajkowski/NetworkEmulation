using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities.Log;
using NetworkUtilities.Utilities;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkUtilities.Network {
    public abstract class ConnectionManager : LogObject {
        private readonly Dictionary<NetworkAddress, TcpClient> _nodesTcpClients;
        private UdpClient _connectionUdpClient;

        protected ConnectionManager(int listeningPort) {
            _nodesTcpClients = new Dictionary<NetworkAddress, TcpClient>();
            ListeningPort = listeningPort;
        }

        public int ListeningPort { get; }

        public bool Online { get; private set; }

        public void StartListening() {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, ListeningPort);
            _connectionUdpClient = new UdpClient(ipEndPoint);

            ListenForConnectionRequests();
            OnUpdateState("Rise and shine.");
        }

        private void ListenForConnectionRequests() {
            Task.Run(async () => {
                using (_connectionUdpClient) {
                    Online = true;
                    while (Online) {
                        var receivedData = await _connectionUdpClient.ReceiveAsync();
                        ConnectToNode(
                            (NetworkAddressSocketPortPair) BinarySerializer.Deserialize(receivedData.Buffer));
                    }
                }
            });
        }

        private void ConnectToNode(NetworkAddressSocketPortPair initializationMessage) {
            var nodeTcpClient = new TcpClient();

            var port = initializationMessage.SocketPort;
            var networkAddress = initializationMessage.NetworkAddress;

            try {
                EstablishConnection(nodeTcpClient, port, networkAddress);
                OnUpdateState($"Received connection request from {networkAddress} - accepted");
                Listen(nodeTcpClient, networkAddress).Start();
            }
            catch (SocketException e) {
                DeleteConnection(networkAddress);
                OnUpdateState($"Connection to {networkAddress} - failed");
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

        private Task Listen(TcpClient nodeTcpClient, NetworkAddress inputNetworkAddress) {
            return new Task(() => {
                try {
                    while (Online) {
                        var receivedObject = Receive(nodeTcpClient.GetStream());
                        HandleReceivedObject(receivedObject, inputNetworkAddress);
                    }
                }
                catch (IOException) {
                    OnUpdateState($"Connection to {inputNetworkAddress} - closed");
                    DeleteConnection(inputNetworkAddress);
                }
            });
        }

        private object Receive(Stream networkStream) {
            return BinarySerializer.DeserializeFromStream(networkStream);
        }

        protected abstract void HandleReceivedObject(object receivedObject, NetworkAddress networkAddress);

        protected void Send(object objectToSend, NetworkAddress outputNetworkAddress) {
            var tcpClient = _nodesTcpClients[outputNetworkAddress];
            var networkStream = tcpClient.GetStream();

            BinarySerializer.SerializeToStream(objectToSend, networkStream);
        }

        public void Dispose() {
            OnUpdateState("Hello darkness my old friend...");
            Online = false;
            _connectionUdpClient.Close();
        }
    }
}