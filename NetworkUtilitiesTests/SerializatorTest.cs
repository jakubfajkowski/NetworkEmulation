using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class SerializatorTest {
        [TestMethod]
        public void SerializeTest() {
            var message = new CableCloudMessage(6321);
            var data = Serializator.Serialize(message);
            var obj = Serializator.Deserialize(data);
            var msg = new CableCloudMessage(1);
            Debug.WriteLine(msg.PortNumber);
            if (obj is CableCloudMessage)
                msg = (CableCloudMessage) obj;
            Assert.AreEqual(message.PortNumber, msg.PortNumber);
            Debug.WriteLine(msg.PortNumber);
        }
    }
}