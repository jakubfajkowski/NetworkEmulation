using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.editor.element;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkEmulationTest {
    [TestClass]
    public class ElementsSerializationTest {
        [TestMethod]
        public void SerializeNetworkNodeSerializableParametersTest() {
            var networkNodeSerializableParameters = new NetworkNodeSerializableParameters {
                Id = 1,
                IpAddress = "127.0.0.1",
                CloudPort = 10000,
                NetworkManagmentSystemPort = 6666,
                NumberOfPorts = 5
            };
            var serialized = XmlSerializer.Serialize(networkNodeSerializableParameters);
        }

        [TestMethod]
        public void SerializeParameters() {
            var parameters = new SerializableParameters {
                Id = 1
            };

            var serialized = XmlSerializer.Serialize(parameters);
        }

        [TestMethod]
        public void SerializeClientNodePictureBox() {
            var clientNodePictureBox = new ClientNodePictureBox {
                Parameters = new ClientNodeSerializableParameters {
                    Id = 1,
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
    }
}