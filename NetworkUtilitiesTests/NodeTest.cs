using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.Serialization;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class NodeTest {
        private readonly Random _random = new Random();

        [TestMethod]
        public void ConnectToCableCloudTest() {
            var listeningTask = StartUdpListener(10000);
            var node = new Node("127.0.0.1", 10000, 6969);

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