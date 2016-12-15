using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class BinarySerializerTest {
        [TestMethod]
        public void SerializeTest() {
            var message = new CableCloudMessage(6321);
            var data = BinarySerializer.Serialize(message);
            var obj = BinarySerializer.Deserialize(data);
            var msg = new CableCloudMessage(1);
            Debug.WriteLine(msg.PortNumber);
            if (obj is CableCloudMessage)
                msg = (CableCloudMessage) obj;
            Assert.AreEqual(message.PortNumber, msg.PortNumber);
            Debug.WriteLine(msg.PortNumber);
        }
    }
}