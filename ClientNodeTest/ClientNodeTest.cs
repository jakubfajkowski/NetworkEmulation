using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using ClientNode;
using NetworkUtilities;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace ClientNodeTest {
    [TestClass]
    public class ClientNodeTest {
        [TestMethod]
        public void SerializeTest() {
            var expectedClientNode = new ClientNode.ClientNode();
            var cn = new PrivateObject(expectedClientNode);

            cn.SetFieldOrProperty("clientName", "Test");

            var serializedClientNode = XmlSerializator.Serialize(expectedClientNode as object);

            var actualClientNode = XmlSerializator.Deserialize(serializedClientNode, typeof(ClientNode.ClientNode)) as ClientNode.ClientNode;
        }
        [TestMethod]
        public void TableSerializeTest() {
            var expectedClientNode = new ClientNode.ClientNode();
            var cn = new PrivateObject(expectedClientNode);

            cn.SetFieldOrProperty("ClientName", "Test");

            expectedClientNode.addClient(3, 4, "Fajka");
            expectedClientNode.addClient(5, 6, "Misiek");


            var serializedClientNode = XmlSerializator.Serialize(expectedClientNode as object);

            var actualClientNode = XmlSerializator.Deserialize(serializedClientNode, typeof(ClientNode.ClientNode)) as ClientNode.ClientNode;
        }
    }
}
