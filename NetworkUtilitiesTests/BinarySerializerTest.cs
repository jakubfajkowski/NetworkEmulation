using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.DataPlane;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class BinarySerializerTest {
        [TestMethod]
        public void SerializeCableCloudMessageTest() {
            var message = new CableCloudMessage(1, AtmCell.Generate(1, 1, "TEST"));
            var data = BinarySerializer.Serialize(message);
            var obj = BinarySerializer.Deserialize(data);
            var msg = new CableCloudMessage(1, AtmCell.Generate(1, 1, "TEST"));
            Debug.WriteLine(msg.PortNumber);
            if (obj is CableCloudMessage)
                msg = (CableCloudMessage) obj;
            Assert.AreEqual(message.PortNumber, msg.PortNumber);
            Debug.WriteLine(msg.PortNumber);
        }

        [TestMethod]
        public void StreamSerializeCableCloudMessageTest() {
            var tcpListener = new TcpListener(IPAddress.Loopback, 10000);
            tcpListener.Start();
            var acceptTask = AcceptTask(tcpListener);

            var client1 = new TcpClient();
            client1.Connect(IPAddress.Loopback, 10000);
            var client2 = acceptTask.Result;


            var expected = new CableCloudMessage(1, AtmCell.Generate(1, 1, "TEST"));

            BinarySerializer.SerializeToStream(expected, client1.GetStream());
            var actual = (CableCloudMessage) BinarySerializer.DeserializeFromStream(client2.GetStream());

            Assert.AreEqual(expected.PortNumber, actual.PortNumber);
        }

        [TestMethod]
        public void StreamSerializeSignallingMessageTest() {
            var tcpListener = new TcpListener(IPAddress.Loopback, 10000);
            tcpListener.Start();
            var acceptTask = AcceptTask(tcpListener);

            var client1 = new TcpClient();
            client1.Connect(IPAddress.Loopback, 10000);
            var client2 = acceptTask.Result;


            var expected = new SignallingMessage();


            BinarySerializer.SerializeToStream(expected, client1.GetStream());
            var actual = (SignallingMessage) BinarySerializer.DeserializeFromStream(client2.GetStream());
            ;

            Assert.AreEqual(expected.SessionId, actual.SessionId);
        }

        private async Task<TcpClient> AcceptTask(TcpListener tcpListener) {
            return await tcpListener.AcceptTcpClientAsync();
        }
    }
}