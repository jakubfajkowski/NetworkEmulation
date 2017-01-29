using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.Network;
using NetworkUtilities;
using NetworkUtilities.Element;
using NetworkUtilities.Serialization;

namespace NetworkNodeTest {
    [TestClass]
    public class NetworkNodeTest {
        [TestMethod]
        public void NetworkNodeSetupTest() {
            var networkNodeSerializableParameters = new NetworkNodeModel {
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = "127.0.0.1",
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
        public void NmsKeepAlive() {
            var networkNodeSerializableParameters = new NetworkNodeModel {
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = "127.0.0.1",
                NetworkManagmentSystemListeningPort = 6666,
                NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
            };
            var nms = new NetworkManagmentSystem();
            var networkNode = new NetworkNode.NetworkNode(networkNodeSerializableParameters);
            Thread.Sleep(10000);
        }

        [TestMethod]
        public void LrMandCcConnectionTest() {
            //CC NIE MA PARAMETRÓW!!!!!!!!!!!!!!!!!!!!!!!!

            //var LRM = new LinkResourceManager(new CommutationTable(),3,300);
            //var LRM2 = new LinkResourceManager(new CommutationTable(), 4, 400);
            //var CC = new ConnectionController(1234,1245);

            //LRM.MessageToSend += (sender, message) => CC.ReceiveMessage(message);
            //LRM2.MessageToSend += (sender, message) => CC.ReceiveMessage(message);
            //CC.MessageToSend += (sender, message) => LRM.ReceiveMessage(message);

            //CC.SendGetLabelsMessage();
            //CC.MessageToSend += (sender, message) => LRM2.ReceiveMessage(message);
            //CC.SendGetLabelsMessage();
        }
    }
}