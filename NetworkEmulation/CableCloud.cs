using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            Task.Run(async () =>
            {
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
            }
            catch (SocketException e) {
                //TODO: Metoda raportująca o błędach.
            }

        }

        private void listenForNodeMessages(TcpClient nodeTcpClient) {
            
        }
    }
}
