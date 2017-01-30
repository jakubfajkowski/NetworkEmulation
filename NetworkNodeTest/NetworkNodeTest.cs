using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.Network;
using NetworkNode;
using NetworkUtilities;
using NetworkUtilities.Element;
using NetworkUtilities.Serialization;

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
                NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
            };

            var serializedParameters = XmlSerializer.Serialize(networkNodeSerializableParameters);

            var args = serializedParameters.Split(' ');

            var joinedArgs = string.Join(" ", args);

            var parameters =
                (NetworkNodeModel)
                XmlSerializer.Deserialize(joinedArgs, typeof(NetworkNodeModel));

            var networkNode = new NetworkNode.NetworkNode(parameters);
            networkNode.Initialize();
        }

        [TestMethod]
        public void NmsKeepAlive() {
            var nms = new NetworkManagmentSystem(6666);
            nms.UpdateState += (sender, state) => Debug.WriteLine(state);
            nms.StartListening();

            var agent = new NetworkNodeAgent(new NetworkAddress(1), "127.0.0.1", 6666);
            agent.UpdateState += (sender, state) => Debug.WriteLine(state);
            agent.Initialize();
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