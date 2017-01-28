using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities.Log;
using NetworkUtilities.Serialization;

namespace NetworkUtilities.Network {
    public abstract class ConnectionManager : LogObject {
        public int ListeningPort { get; }
        private readonly Dictionary<NetworkAddress, TcpClient> _nodesTcpClients;
        private UdpClient _connectionUdpClient;

        protected ConnectionManager(int listeningPort) {
            _nodesTcpClients = new Dictionary<NetworkAddress, TcpClient>();
            ListeningPort = listeningPort;
        }

        public bool Online { get; private set; }

        public virtual void Initialize() {
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
                        EstabilishNodeConnection((NetworkAddressSocketPortPair) BinarySerializer.Deserialize(receivedData.Buffer));
                    }
                }
            });
        }

        private void EstabilishNodeConnection(NetworkAddressSocketPortPair initializationMessage) {
            var nodeTcpClient = new TcpClient();

            var port = initializationMessage.SocketPort;
            var networkAddress = initializationMessage.NetworkAddress;

            try {
                ConnectWithClient(nodeTcpClient, port, networkAddress);
                OnUpdateState($"Connected to Node {networkAddress} on TCP port: {port}");
                Listen(nodeTcpClient, networkAddress).Start();
            }
            catch (SocketException e) {
                OnUpdateState(e.Message);
            }
        }

        private void ConnectWithClient(TcpClient nodeTcpClient, int port, NetworkAddress networkAddress) {
            nodeTcpClient.Connect(IPAddress.Loopback, port);
            _nodesTcpClients.Add(networkAddress, nodeTcpClient);
        }

        protected void DisconnectClient(NetworkAddress networkAddress) {
            _nodesTcpClients.Remove(networkAddress);
        }

        private Task Listen(TcpClient nodeTcpClient, NetworkAddress inputNetworkAddress) {
            return new Task(() => {
                while (Online) {
                    var receivedObject = ReceiveObject(nodeTcpClient.GetStream());
                    HandleReceivedObject(receivedObject, inputNetworkAddress);
                }
            });
        }

        protected abstract void HandleReceivedObject(object receivedObject, NetworkAddress networkAddress);

        protected void SendObject(object objectToSend, NetworkAddress outputNetworkAddress) {
            var tcpClient = _nodesTcpClients[outputNetworkAddress];
            var networkStream = tcpClient.GetStream();

            BinarySerializer.SerializeToStream(objectToSend, networkStream);
        }

        protected object ReceiveObject(Stream networkStream) {
            return BinarySerializer.DeserializeFromStream(networkStream);
        }

        public void Dispose() {
            OnUpdateState("It's gettin' dark... To dark to see.");
            _connectionUdpClient.Close();
        }
    }
}
