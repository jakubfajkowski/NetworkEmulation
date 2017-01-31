using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities.ManagementPlane;
using NetworkUtilities.Network.NetworkNode;
using NetworkUtilities.Utilities;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkNodeTest {
    [TestClass]
    public class NetworkNodeTest {
        [TestMethod]
        public void NetworkNodeSetupTest() {
            var networkNodeSerializableParameters = new NetworkNodeModel {
                NetworkAddress = new NetworkAddress(1),
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = "127.0.0.1",
                NetworkManagmentSystemListeningPort = 6666,
                SignallingCloudListeningPort = PortRandomizer.RandomFreePort()
            };

            var serializedParameters = XmlSerializer.Serialize(networkNodeSerializableParameters);

            var args = serializedParameters.Split(' ');

            var joinedArgs = string.Join(" ", args);

            var parameters =
                (NetworkNodeModel)
                XmlSerializer.Deserialize(joinedArgs, typeof(NetworkNodeModel));

            var networkNode = new NetworkNode(parameters);
            networkNode.Initialize();
        }

        [TestMethod]
        public void NmsKeepAlive() {
            var nms = new NetworkManagementSystem(6666);
            nms.UpdateState += (sender, state) => Debug.WriteLine(state);
            nms.StartListening();

            var agent = new NetworkNodeAgent(new NetworkAddress(1), "127.0.0.1", 6666);
            agent.UpdateState += (sender, state) => Debug.WriteLine(state);
            agent.Initialize();
            Thread.Sleep(10000);
        }
    }
}