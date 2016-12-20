using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class NodeTest {
        private readonly Random _random = new Random();

        [TestMethod]
        public void ConnectToCableCloudTest() {
            Node.CableCloudListeningPort = _random.Next(1024, 65535);

            var listeningTask = StartUdpListener(Node.CableCloudListeningPort);
            var node = new Node();

            listeningTask.Wait();

            Assert.IsFalse(listeningTask.Result == null);
            Assert.AreEqual(node.CableCloudDataPort, listeningTask.Result.Value);
        }

        private Task<int?> StartUdpListener(int port) {
            var udpListener = new UdpClient(port);

            return Task.Run(async () => {
                var receivedData = await udpListener.ReceiveAsync();
                return BinarySerializer.Deserialize(receivedData.Buffer) as int?;
            });
        }
    }
}