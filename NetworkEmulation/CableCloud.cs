using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkEmulation {
    public class CableCloud {
        private readonly UdpClient udpClient;

        public CableCloud() {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 10000);
            this.udpClient = new UdpClient(ipEndPoint);

            startListening();
        }

        private void startListening() {
            Task.Run(async () =>
            {
                using (udpClient) {
                    while (true) {
                        var receivedResults = await udpClient.ReceiveAsync();
                        Console.Write(Encoding.ASCII.GetString(receivedResults.Buffer));
                    }
                }
            });
        }
    }
}
