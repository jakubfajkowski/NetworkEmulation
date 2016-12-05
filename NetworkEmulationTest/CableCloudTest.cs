using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkEmulation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkEmulationTest {
    [TestClass]
    public class CableCloudTest {
        private UdpClient udpClient;
        [TestMethod]
        public void CableCloudConnectionTest() {
            CableCloud cableCloud = new CableCloud();

            UdpClient udpClient = new UdpClient();

            byte[] bytesToSend = Encoding.ASCII.GetBytes("Wiadomosc");

            var ipEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
            udpClient.Send(bytesToSend, bytesToSend.Length, ipEndpoint);
        }
    }
}
