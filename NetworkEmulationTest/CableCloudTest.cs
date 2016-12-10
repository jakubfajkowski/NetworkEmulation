using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation;
using NetworkUtilities;

namespace NetworkEmulationTest {
    [TestClass]
    public class CableCloudTest {
        IPEndPoint cableCloudIpEndpoint = new IPEndPoint(IPAddress.Loopback, 10000);
        private byte[] bytesToSend;
        private byte[] bytesRecieved;

        [TestMethod]
        public void CableCloudBindEndpointTest() {
            CableCloud cableCloud = new CableCloud();
            int port = 10001;
            
            var listenerTask = startTcpListener(port, listen);

            connectToCableCloud(port);
            listenerTask.Wait();

            var nodesTcpClients =
                (Dictionary<int, TcpClient>)new PrivateObject(cableCloud).GetField("_nodesTcpClients");
            Assert.AreEqual(1, nodesTcpClients.Count);
        }

        [TestMethod]
        public void CableCloudConnectNodeTest() {
            CableCloud cableCloud = new CableCloud();
            while (!cableCloud.Online) ;
            Node node = new Node();
            while (!node.isOnline()) ;

            var nodesTcpClients = (Dictionary<int, TcpClient>)new PrivateObject(cableCloud).GetField("_nodesTcpClients");
            Assert.AreEqual(1, nodesTcpClients.Count);
        }

        [TestMethod]
        public void CableCloudPassMessageTest() {
            CableCloud cableCloud = new CableCloud();
            cableCloud.OnUpdateState += (sender, state) => Console.WriteLine(state);
            int port1 = 10001;
            int port2 = 10002;
            int port3 = 10003;

            var output = new SocketNodePortPair(1, port1);
            var input1 = new SocketNodePortPair(1, port2);
            var input2 = new SocketNodePortPair(1, port3);

            cableCloud.AddLink(input1, output);
            cableCloud.AddLink(input2, output);
            bytesToSend = CableCloudMessage.serialize(createCableCloudMessage(1, 100));

            var listenerTask1 = startTcpListener(port1, recieveMessage);
            connectToCableCloud(port1);
            var listenerTask2 = startTcpListener(port2, sendMessage);
            connectToCableCloud(port2);
            var listenerTask3 = startTcpListener(port3, sendMessage);
            connectToCableCloud(port3);

            Task.WaitAll(listenerTask1, listenerTask2, listenerTask3);

            for (int i = 0; i < bytesToSend.Length; i++) {
                Assert.AreEqual(bytesToSend[i], bytesRecieved[i]);
            }
        }

        private static CableCloudMessage createCableCloudMessage(int linkNumber, int atmCellsNumber) {
            var cableCloudMessage = new CableCloudMessage(linkNumber);
            for (int i = 0; i < atmCellsNumber; i++) {
                cableCloudMessage.add(new ATMCell(1, 1, new byte[48]));
            }

            return cableCloudMessage;
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
