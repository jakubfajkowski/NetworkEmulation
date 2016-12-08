using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation;
using NetworkUtilities;

namespace NetworkEmulationTest {
    [TestClass]
    public class CableCloudTest {
        IPEndPoint cableCloudIpEndpoint = new IPEndPoint(IPAddress.Loopback, 10000);
        private byte[] bytesToSend = CableCloudMessage.serialize(new CableCloudMessage(1));
        private byte[] bytesRecieved;

        [TestMethod]
        public void CableCloudBindEndpointTest() {
            CableCloud cableCloud = new CableCloud();
            int port = 10001;
            
            var listenerTask = startTcpListener(port, listen);

            connectToCableCloud(port);
            listenerTask.Wait();

            var nodesTcpClients =
                (Dictionary<int, TcpClient>)new PrivateObject(cableCloud).GetField("nodesTcpClients");
            Assert.AreEqual(1, nodesTcpClients.Count);
        }

        [TestMethod]
        public void CableCloudConnectNodeTest() {
            CableCloud cableCloud = new CableCloud();
            while (!cableCloud.isOnline()) ;
            Node node = new Node(1, 10001);
            while (!node.isOnline()) ;

            var nodesTcpClients = (Dictionary<int, TcpClient>)new PrivateObject(cableCloud).GetField("nodesTcpClients");
            Assert.AreEqual(1, nodesTcpClients.Count);
        }

        [TestMethod]
        public void CableCloudPassMessageTest() {
            CableCloud cableCloud = new CableCloud();
            int port1 = 10001;
            int port2 = 10002;
            cableCloud.addLink(1, 10001);

            var listenerTask1 = startTcpListener(port1, recieveMessage);
            connectToCableCloud(port1);
            var listenerTask2 = startTcpListener(port2, sendMessage);
            connectToCableCloud(port2);

            Task.WaitAll(listenerTask1, listenerTask2);

            for (int i = 0; i < bytesToSend.Length; i++) {
                Assert.AreEqual(bytesToSend[i], bytesRecieved[i]);
            }
            
        }

        private void connectToCableCloud(int port) {
            UdpClient udpClient = new UdpClient();

            byte[] bytesToSend = BitConverter.GetBytes(port);
            udpClient.Send(bytesToSend, bytesToSend.Length, cableCloudIpEndpoint);
        }

        private Task startTcpListener(int port, Func<TcpListener, Task> function) {
            var nodeIpEndpoint = new IPEndPoint(IPAddress.Loopback, port);
            var tcpListener = new TcpListener(nodeIpEndpoint);
            tcpListener.Start();
            return Task.Run(async () => {
                await function(tcpListener);
            });
        }

        private Task listen(TcpListener tcpListener) {
            return Task.Run(async () => {
                await tcpListener.AcceptTcpClientAsync();
            });
        }

        private Task sendMessage(TcpListener tcpListener) {
            return Task.Run(async () => {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                tcpClient.GetStream().Write(bytesToSend, 0, bytesToSend.Length);
            });
        }

        private Task recieveMessage(TcpListener tcpListener) {
            return Task.Run(async () => {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                bytesRecieved = new byte[bytesToSend.Length];
                await tcpClient.GetStream().ReadAsync(bytesRecieved, 0, bytesRecieved.Length);
            });
        }
    }
}
