using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities.Log;
using NetworkUtilities.Serialization;

namespace NetworkUtilities {
    public class ConnectionManager : LogObject {
        protected readonly Dictionary<int, TcpClient> NodesTcpClients;
        private UdpClient _connectionUdpClient;

        protected ConnectionManager() {
            NodesTcpClients = new Dictionary<int, TcpClient>();

            Start();
        }

        public bool Online { get; private set; }

        private void Start() {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 10000);
            _connectionUdpClient = new UdpClient(ipEndPoint);

            ListenForConnectionRequests();
        }

        private void ListenForConnectionRequests() {
            Task.Run(async () => {
                using (_connectionUdpClient) {
                    Online = true;
                    while (true) {
                        var receivedData = await _connectionUdpClient.ReceiveAsync();
                        EstabilishNodeConnection((int) BinarySerializer.Deserialize(receivedData.Buffer));
                    }
                }
            });
        }

        private void EstabilishNodeConnection(int port) {
            var nodeTcpClient = new TcpClient();
            try {
                nodeTcpClient.Connect(IPAddress.Loopback, port);
                NodesTcpClients.Add(port, nodeTcpClient);
                UpdateState("Connected to Node on TCP port: " + port);
                Listen(nodeTcpClient, port).Start();
            }
            catch (SocketException e) {
                UpdateState(e.Message);
            }
        }

        protected virtual Task Listen(TcpClient nodeTcpClient, int port) {
            return null;
        }

        protected void SendBytes(byte[] data, TcpClient tcpClient) {
            tcpClient.GetStream().Write(data, 0, data.Length);
        }

        public void Dispose() {
            UpdateState("Shutting down.");
            _connectionUdpClient.Close();
        }
    }
}
