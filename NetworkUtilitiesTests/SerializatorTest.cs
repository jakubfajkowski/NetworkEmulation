using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;

namespace NetworkUtilitiesTests
{
    [TestClass]
    public class SerializatorTest
    {
        [TestMethod]
        public void serializeTest()
        {
            CableCloudMessage message = new CableCloudMessage(6321);
            byte[] data = Serializator.Serialize(message);
            object obj = Serializator.Deserialize(data);
            CableCloudMessage msg = new CableCloudMessage(1);
            Debug.WriteLine(msg.portNumber);
            if (obj is CableCloudMessage)
            {
                msg = (CableCloudMessage) obj;
            }
            Assert.AreEqual(message.portNumber, msg.portNumber);
            Debug.WriteLine(msg.portNumber);
        }
    }
}