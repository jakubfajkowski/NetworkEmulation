using System.Collections.Generic;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.editor.element;
using NetworkEmulation.network;
using NetworkEmulation.network.element;
using NetworkUtilities;
using NetworkUtilities.element;
using UniqueId = NetworkUtilities.UniqueId;

namespace NetworkEmulationTest {
    [TestClass]
    public class ElementsSerializationTest {
        [TestMethod]
        public void SerializeNetworkNodeSerializableParametersTest() {
            var networkNodeSerializableParameters = new NetworkNodeSerializableParameters {
                IpAddress = "127.0.0.1",
                CloudPort = 10000,
                NetworkManagmentSystemPort = 6666,
                NumberOfPorts = 5
            };
            var serialized = XmlSerializer.Serialize(networkNodeSerializableParameters);
        }

        [TestMethod]
        public void SerializeClientNodePictureBox() {
            var clientNodePictureBox = new ClientNodePictureBox {
                Parameters = new ClientNodeSerializableParameters {
                    ClientName = "Janusz",
                    ClientTable =
                        new List<ClientTableRow>(new[]
                            {new ClientTableRow("clientName", 1, 2, 3), new ClientTableRow("clientName2", 1, 2, 3)}),
                    CloudPort = 10000,
                    IpAddress = "localhost"
                }
            };

            var serialized = XmlSerializer.Serialize(clientNodePictureBox);

            var deserialized = new ClientNodePictureBox();
            XmlSerializer.Deserialize(deserialized, serialized);
        }

        [TestMethod]
        public void SerializeNetworkNodePictureBox() {
            var networkNodePictureBox = new NetworkNodePictureBox {
                Parameters = new NetworkNodeSerializableParameters {
                    CloudPort = 10000,
                    IpAddress = "localhost",
                    NetworkManagmentSystemPort = 6666,
                    NumberOfPorts = 8
                }
            };

            var serialized = XmlSerializer.Serialize(networkNodePictureBox);

            var deserialized = new NetworkNodePictureBox();
            XmlSerializer.Deserialize(deserialized, serialized);
        }

        [TestMethod]
        public void SerializeLink() {
            var link = new Link {
                Parameters = new LinkSerializableParameters {
                    BeginNodePictureBoxId = 1,
                    EndNodePictureBoxId = 2,
                    InputNodePortPair = new SocketNodePortPair(3,4),
                    OutputNodePortPair = new SocketNodePortPair(5,6)
                }
            };

            var serialized = XmlSerializer.Serialize(link);

            var deserialized = new Link();
            XmlSerializer.Deserialize(deserialized, serialized);
        }

        [TestMethod]
        public void SerializeConnection() { 
            var connection = new Connection {
                Parameters = new ConnectionSerializableParameters {
                    LinksIds = new List<UniqueId>(new[] {UniqueId.Generate(), UniqueId.Generate(), UniqueId.Generate()})
                }
            };

            var serialized = XmlSerializer.Serialize(connection);

            var deserialized = new Connection();
            XmlSerializer.Deserialize(deserialized, serialized);
        }

        [TestMethod]
        public void UniqueIdTest() {
            var expected = UniqueId.Generate();
            var serialized = XmlSerializer.Serialize(expected);

            var actual = XmlSerializer.Deserialize(serialized, typeof(UniqueId));
            var notExpected = UniqueId.Generate();

            Assert.AreEqual(expected, actual);
            Assert.AreNotEqual(notExpected, actual);
        }
    }
}