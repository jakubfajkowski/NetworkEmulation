using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkEmulation {
    public class CableCloud {
        private readonly UdpClient connectionUdpClient;
        private Dictionary<int, TcpClient> nodesTcpClients;

        public CableCloud() {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 10000);
            this.connectionUdpClient = new UdpClient(ipEndPoint);

            this.nodesTcpClients = new Dictionary<int, TcpClient>();

            listenForConnectionRequests();
        }

        private void listenForConnectionRequests() {
            Task.Run(async () => {
                using (connectionUdpClient) {
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
                listenForNodeMessages(nodeTcpClient);
                Console.Write("Connected to Node on port: " + port);
            }
            catch (SocketException e) {
                MessageBox.Show(e.Message, "Cable Cloud Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Console.Write(ms.ToArray());
                    ms.Seek(0, SeekOrigin.Begin);
                }
            }
            });
        }
    }
}
