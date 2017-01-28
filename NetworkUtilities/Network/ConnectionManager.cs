using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities.Log;
using NetworkUtilities.Serialization;

namespace NetworkUtilities.Network {
    public abstract class ConnectionManager : LogObject {
        private readonly Dictionary<int, TcpClient> _nodesTcpClients;
        private UdpClient _connectionUdpClient;

        protected ConnectionManager(int port) {
            _nodesTcpClients = new Dictionary<int, TcpClient>();

            Start(port);
        }

        public bool Online { get; private set; }

        private void Start(int port) {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, port);
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
                        EstabilishNodeConnection((int) BinarySerializer.Deserialize(receivedData.Buffer));
                    }
                }
            });
        }

        private void EstabilishNodeConnection(int port) {
            var nodeTcpClient = new TcpClient();

            try {
                ConnectWithClient(nodeTcpClient, port);
                OnUpdateState("Connected to Node on TCP port: " + port);
                Listen(nodeTcpClient, port).Start();
            }
            catch (SocketException e) {
                OnUpdateState(e.Message);
            }
        }

        private void ConnectWithClient(TcpClient nodeTcpClient, int port) {
            nodeTcpClient.Connect(IPAddress.Loopback, port);
            _nodesTcpClients.Add(port, nodeTcpClient);
        }

        protected void DisconnectClient(int port) {
            _nodesTcpClients.Remove(port);
        }

        private Task Listen(TcpClient nodeTcpClient, int inputPort) {
            return new Task(() => {
                while (Online) {
                    var receivedObject = ReceiveObject(nodeTcpClient.GetStream());
                    HandleReceivedObject(receivedObject, inputPort);
                }
            });
        }

        protected abstract void HandleReceivedObject(object receivedObject, int inputPort);

        protected void SendObject(object objectToSend, int outputPort) {
            var tcpClient = _nodesTcpClients[outputPort];
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
