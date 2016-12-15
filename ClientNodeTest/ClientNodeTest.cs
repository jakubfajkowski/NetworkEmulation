using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;

namespace ClientNodeTest {
    [TestClass]
    public class ClientNodeTest {
        [TestMethod]
        public void SerializeTest() {
            var expectedClientNode = new ClientNode.ClientNode();
            var cn = new PrivateObject(expectedClientNode);

            cn.SetFieldOrProperty("ClientName", "Test");

            var serializedClientNode = XmlSerializer.Serialize(expectedClientNode);

            var actualClientNode =
                XmlSerializer.Deserialize(serializedClientNode, typeof(ClientNode.ClientNode)) as
                    ClientNode.ClientNode;
        }

        [TestMethod]
        public void TableSerializeTest() {
            var expectedClientNode = new ClientNode.ClientNode();
            var cn = new PrivateObject(expectedClientNode);

            cn.SetFieldOrProperty("ClientName", "Test");

            expectedClientNode.AddClient("Fajka", 1, 3, 4);
            expectedClientNode.AddClient("Misiek", 2, 5, 6);


            var serializedClientNode = XmlSerializer.Serialize(expectedClientNode);

            var actualClientNode =
                XmlSerializer.Deserialize(serializedClientNode, typeof(ClientNode.ClientNode)) as
                    ClientNode.ClientNode;
        }
    }
}