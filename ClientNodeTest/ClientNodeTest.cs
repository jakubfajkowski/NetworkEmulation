using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClientNode;
using NetworkUtilities;
using System.Collections.Generic;

namespace ClientNodeTest {
    [TestClass]
    public class ClientNodeTest {
        [TestMethod]
        public void SerializeTest() {
            var expectedClientNode = new ClientNode.ClientNode();
            var cn = new PrivateObject(expectedClientNode);

            cn.SetFieldOrProperty("clientName", "Test");

            var serializedClientNode = XmlSerializator.Serialize(expectedClientNode);

            var actualClientNode = new ClientNode.ClientNode();
            XmlSerializator.Deserialize(actualClientNode, serializedClientNode);

        }
        [TestMethod]
        public void TableSerializeTest() {
            var expectedClientNode = new ClientNode.ClientNode();
            var cn = new PrivateObject(expectedClientNode);

            cn.SetFieldOrProperty("clientName", "Test");

            expectedClientNode.addClient(3, 4, "Fajka");
            expectedClientNode.addClient(5, 6, "Misiek");
            

            var serializedClientNode = XmlSerializator.Serialize(expectedClientNode);

            var actualClientNode = new ClientNode.ClientNode();
            XmlSerializator.Deserialize(actualClientNode, serializedClientNode);

        }
    }
}
