using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.Editor.Element;
using NetworkEmulation.Network;
using NetworkEmulation.Network.Element;
using NetworkUtilities;
using NetworkUtilities.Element;
using NetworkUtilities.Serialization;

namespace NetworkEmulationTest {
    [TestClass]
    public class ElementsSerializationTest {
        [TestMethod]
        public void SerializeNetworkNodeSerializableParametersTest() {
            var networkNodeSerializableParameters = new NetworkNodeModel {
                IpAddress = "127.0.0.1",
                CableCloudListeningPort = 10000,
                NetworkManagmentSystemListeningPort = 6666,
                NumberOfPorts = 5
            };
            var serialized = XmlSerializer.Serialize(networkNodeSerializableParameters);
        }

        [TestMethod]
        public void SerializeClientNodePictureBox() {
            var clientNodePictureBox = new NetworkEmulation.Editor.Element.ClientNodeView {
                Parameters = new ClientNodeModel {
                    ClientName = "Janusz",
                    ClientTable =
                        new List<ClientTableRow>(new[]
                            {new ClientTableRow("clientName", 1, 2, 3), new ClientTableRow("clientName2", 1, 2, 3)}),
                    CableCloudListeningPort = 10000,
                    IpAddress = "localhost"
                }
            };

            var serialized = XmlSerializer.Serialize(clientNodePictureBox);

            var deserialized = new NetworkEmulation.Editor.Element.ClientNodeView();
            XmlSerializer.Deserialize(deserialized, serialized);
        }

        [TestMethod]
        public void SerializeNetworkNodePictureBox() {
            var networkNodePictureBox = new NetworkEmulation.Editor.Element.NetworkNodeView {
                Parameters = new NetworkNodeModel {
                    CableCloudListeningPort = 10000,
                    IpAddress = "localhost",
                    NetworkManagmentSystemListeningPort = 6666,
                    NumberOfPorts = 8
                }
            };

            var serialized = XmlSerializer.Serialize(networkNodePictureBox);

            var deserialized = new NetworkEmulation.Editor.Element.NetworkNodeView();
            XmlSerializer.Deserialize(deserialized, serialized);
        }

        [TestMethod]
        public void SerializeLink() {
            var link = new Link {
                Parameters = new LinkModel {
                    BeginNodeViewId = UniqueId.Generate(),
                    EndNodeViewId = UniqueId.Generate(),
                    InputNodePortPair = new SocketNodePortPair(3, 4),
                    OutputNodePortPair = new SocketNodePortPair(5, 6)
                }
            };

            var serialized = XmlSerializer.Serialize(link);

            var deserialized = new Link();
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