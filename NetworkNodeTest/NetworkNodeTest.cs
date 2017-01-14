using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.Network;
using NetworkUtilities;
using NetworkUtilities.Element;
using NetworkUtilities.Serialization;
using NetworkNode;
using NetworkUtilities.ControlPlane;

namespace NetworkNodeTest {
    [TestClass]
    public class NetworkNodeTest {
        [TestMethod]
        public void NetworkNodeSetupTest() {
            var networkNodeSerializableParameters = new NetworkNodeModel {
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = "127.0.0.1",
                CableCloudDataPort = PortRandomizer.RandomFreePort(),
                NetworkManagmentSystemListeningPort = 6666,
                NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
            };

            var serializedParameters = XmlSerializer.Serialize(networkNodeSerializableParameters);

            var args = serializedParameters.Split(' ');

            var joinedArgs = string.Join(" ", args);

            var parameters =
                (NetworkNodeModel)
                XmlSerializer.Deserialize(joinedArgs, typeof(NetworkNodeModel));

            var networkNode = new NetworkNode.NetworkNode(parameters);
        }

        [TestMethod]
        public void NMSKeepAlive() {
            var networkNodeSerializableParameters = new NetworkNodeModel {
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = "127.0.0.1",
                CableCloudDataPort = PortRandomizer.RandomFreePort(),
                NetworkManagmentSystemListeningPort = 6666,
                NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
            };
            var nms = new NetworkManagmentSystem();
            var networkNode = new NetworkNode.NetworkNode(networkNodeSerializableParameters);
            Thread.Sleep(10000);
        }

        [TestMethod]
        public void LRMandCCConnectionTest()
        {
            var LRM = new LinkResourceManager(new CommutationTable(),3,300);
            var LRM2 = new LinkResourceManager(new CommutationTable(), 4, 400);
            var CC = new ConnectionController(1234,1245);

            LRM.OnMessageToSend += (sender, message) => CC.RecieveMessage(message);
            LRM2.OnMessageToSend += (sender, message) => CC.RecieveMessage(message);
            CC.OnMessageToSend += (sender, message) => LRM.RecieveMessage(message);

            CC.SendGetLabelsMessage();
            CC.OnMessageToSend += (sender, message) => LRM2.RecieveMessage(message);
            CC.SendGetLabelsMessage();
        }


    }
}