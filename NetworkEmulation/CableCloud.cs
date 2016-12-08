using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities;

namespace NetworkEmulation {
    public class CableCloud {
        private bool online;
        private readonly UdpClient connectionUdpClient;
        private readonly Dictionary<int, TcpClient> nodesTcpClients;
        private Dictionary<int, int> linkNumberToPortNumberDictionary;

        public CableCloud() {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 10000);
            connectionUdpClient = new UdpClient(ipEndPoint);

            nodesTcpClients = new Dictionary<int, TcpClient>();
            linkNumberToPortNumberDictionary = new Dictionary<int, int>();

            listenForConnectionRequests();
        }

        private void listenForConnectionRequests() {
            Task.Run(async () => {
                using (connectionUdpClient) {
                    online = true;
                    while (true) {
                        var receivedData = await connectionUdpClient.ReceiveAsync();
                        estabilishNodeConnection(BitConverter.ToInt32(receivedData.Buffer, 0));
                    }
                }
            });
        }

        private void estabilishNodeConnection(int port) {
            var nodeTcpClient = new TcpClient();
            try {
                nodeTcpClient.Connect(IPAddress.Loopback, port);
                nodesTcpClients.Add(port, nodeTcpClient);
                Console.Write("Connected to Node on port: " + port);
                listenForNodeMessages(nodeTcpClient);
            }
            catch (SocketException e) {
                Console.WriteLine(e.Message);
            }

        }

        private void listenForNodeMessages(TcpClient nodeTcpClient) {
            Task.Run(async () => {
                using (NetworkStream ns = nodeTcpClient.GetStream()) {
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[1024];

                while (true) {
                    int bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead <= 0)
                        break;
                    ms.Write(buffer, 0, bytesRead);
                    passCableCloudMessage(CableCloudMessage.deserialize(ms.ToArray()));
                    ms.Seek(0, SeekOrigin.Begin);
                }
            }
            });
        }

        private void passCableCloudMessage(CableCloudMessage cableCloudMessage) {
            try {
                var portNumber = linkNumberToPortNumberDictionary[cableCloudMessage.linkNumber];
                var tcpClient = nodesTcpClients[portNumber];

                sendBytes(CableCloudMessage.serialize(cableCloudMessage), tcpClient);
            }
            catch (KeyNotFoundException e) {
                Console.WriteLine("Link number: " + cableCloudMessage.linkNumber + " is offline.");
            }
        }

        private void sendBytes(byte[] data, TcpClient tcpClient) {
            tcpClient.GetStream().Write(data, 0, data.Length);
        }

        public void addLink(int linkNumber, int portNumber) {
            linkNumberToPortNumberDictionary.Add(linkNumber, portNumber);
        }

        public void removeLink(int linkNumber) {
            linkNumberToPortNumberDictionary.Remove(linkNumber);
        }

        public bool isOnline() {
            return online;
        }
    }
}
