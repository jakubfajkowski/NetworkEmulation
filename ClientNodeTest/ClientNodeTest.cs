using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClientNode;
using NetworkUtilities;

namespace ClientNodeTest {
    [TestClass]
    public class ClientNodeTest {
        [TestMethod]
        public void SerializeTest() {
            var expectedClientNode = new ClientNode.ClientNode();
            var cn = new PrivateObject(expectedClientNode);

            cn.SetFieldOrProperty("clientName", "Test");
            cn.SetFieldOrProperty("vpi", 1);
            cn.SetFieldOrProperty("vci", 2);
            cn.SetFieldOrProperty("portNumber", 10000);

            var serializedClientNode = XmlSerializator.Serialize(expectedClientNode);

            var actualClientNode = new ClientNode.ClientNode();
            XmlSerializator.Deserialize(actualClientNode, serializedClientNode);

        }
    }
}
