using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.Serialization;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class BinarySerializerTest {
        [TestMethod]
        public void SerializeTest() {
            var message = new CableCloudMessage(1, AtmCell.Generate(1,1,"TEST"));
            var data = BinarySerializer.Serialize(message);
            var obj = BinarySerializer.Deserialize(data);
            var msg = new CableCloudMessage(1, AtmCell.Generate(1, 1, "TEST"));
            Debug.WriteLine(msg.PortNumber);
            if (obj is CableCloudMessage)
                msg = (CableCloudMessage)obj;
            Assert.AreEqual(message.PortNumber, msg.PortNumber);
            Debug.WriteLine(msg.PortNumber);
        }

        [TestMethod]
        public void StreamSerializeTest() {
            var tcpListener = new TcpListener(IPAddress.Loopback, 10000);
            tcpListener.Start();
            var acceptTask = AcceptTask(tcpListener);

            var client1 = new TcpClient();
            client1.Connect(IPAddress.Loopback, 10000);
            var client2 = acceptTask.Result;


            var expected = new CableCloudMessage(1, AtmCell.Generate(1, 1, "TEST"));

            var recieveTask = BinarySerializer.DeserializeFromStream(client2.GetStream());

            BinarySerializer.SerializeToStream(expected, client1.GetStream());
            var actual = (CableCloudMessage) recieveTask;

            Assert.AreEqual(expected.PortNumber, actual.PortNumber);
        }

        private async Task<TcpClient> AcceptTask(TcpListener tcpListener) {
            return await tcpListener.AcceptTcpClientAsync();
        }
    }
}