using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkNodeTest {
    [TestClass]
    public class NetworkNodeTest {
        [TestMethod]
        public void NetworkNodeSetupTest() {
            var networkNodeSerializableParameters = new NetworkNodeSerializableParameters {
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = "127.0.0.1",
                CableCloudDataPort = PortRandomizer.RandomFreePort(),
                NetworkManagmentSystemListeningPort = 6666,
                NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
            };

            string serializedParameters = XmlSerializer.Serialize(networkNodeSerializableParameters);

            string[] args = serializedParameters.Split(' ');

            string joinedArgs = string.Join(" ", args);

            var parameters = (NetworkNodeSerializableParameters)XmlSerializer.Deserialize(joinedArgs, typeof(NetworkNodeSerializableParameters));

            var networkNode = new NetworkNode.NetworkNode(parameters);
        }

       
    }
}