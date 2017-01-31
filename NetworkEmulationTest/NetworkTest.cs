using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.Workplace.Element;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.DataPlane;
using NetworkUtilities.ManagementPlane;
using NetworkUtilities.Network.ClientNode;
using NetworkUtilities.Network.NetworkNode;
using NetworkUtilities.Utilities;

namespace NetworkEmulationTest {
    [TestClass]
    public class NetworkTest {
        [TestMethod]
        public void CreateBasicNetworkTest() {
            var localhost = "127.0.0.1";
            var maxAtmCellsInCableCloudMessage = 100;

            var portA = 1;
            var port1 = 2;
            var port2 = 3;
            var port3 = 4;
            var port4 = 5;
            var portB = 6;

            var cableCloud = new CableCloud(10000);
            cableCloud.UpdateState += (sender, state) => Console.WriteLine("CC: " + state);
            cableCloud.StartListening();

            var signallingCloud = new SignallingCloud(20000);
            signallingCloud.UpdateState += (sender, state) => Console.WriteLine("SC: " + state);
            signallingCloud.StartListening();

            var nms = new NetworkManagementSystem(6666);
            nms.UpdateState += (sender, state) => Console.WriteLine("NMS:" + state);
            nms.StartListening();

            Thread.Sleep(1000);

            var nameServer = new NameServer(localhost, 20000);
            nameServer.UpdateState += (sender, state) => Console.WriteLine("NS " + state);
            nameServer.Initialize();

            var sspcs1 = new StepByStepPathComputationServer(
                new NetworkAddress(1),
                localhost,
                20000
            );
            sspcs1.UpdateState += (sender, state) => Console.WriteLine("SSPCS1: " + state);
            sspcs1.Initialize();

            var sspcs2 = new StepByStepPathComputationServer(
                new NetworkAddress(2),
                localhost,
                20000
            );
            sspcs2.UpdateState += (sender, state) => Console.WriteLine("SSPCS2: " + state);
            sspcs2.Initialize();

            var clientNodeA = new ClientNode(new ClientNodeModel {
                NetworkAddress = new NetworkAddress("1.1"),
                MaxAtmCellsNumberInCableCloudMessage = maxAtmCellsInCableCloudMessage,
                ClientName = "A",
                CableCloudListeningPort = 10000,
                SignallingCloudListeningPort = 20000,
                IpAddress = localhost
            });
            //clientNodeA.OnMessageReceived += (sender, state) => Console.WriteLine(state);
            clientNodeA.UpdateState += (sender, state) => Console.WriteLine("ClientNode A: " + state);
            clientNodeA.Initialize();
            nameServer.UpdateDirectory("A", new SubnetworkPointPool(clientNodeA.NetworkAddress.Append(1)));

            var clientNodeB = new ClientNode(new ClientNodeModel {
                NetworkAddress = new NetworkAddress("2.2"),
                MaxAtmCellsNumberInCableCloudMessage = maxAtmCellsInCableCloudMessage,
                ClientName = "B",
                CableCloudListeningPort = 10000,
                SignallingCloudListeningPort = 20000,
                IpAddress = localhost
            });
            //clientNodeB.OnMessageReceived += (sender, state) => Console.WriteLine(state);
            clientNodeB.UpdateState += (sender, state) => Console.WriteLine("ClientNode B: " + state);
            clientNodeB.Initialize();
            nameServer.UpdateDirectory("B", new SubnetworkPointPool(clientNodeB.NetworkAddress.Append(1)));

            var networkNode1 = new NetworkNode(new NetworkNodeModel {
                NetworkAddress = new NetworkAddress("1.2"),
                MaxAtmCellsNumberInCableCloudMessage = maxAtmCellsInCableCloudMessage,
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = localhost,
                NetworkManagmentSystemListeningPort = 6666,
                SignallingCloudListeningPort = 20000
            });
            networkNode1.UpdateState += (sender, state) => Console.WriteLine("NetworkNode 1.2: " + state);
            networkNode1.Initialize();

            var networkNode2 = new NetworkNode(new NetworkNodeModel {
                NetworkAddress = new NetworkAddress("2.1"),
                MaxAtmCellsNumberInCableCloudMessage = maxAtmCellsInCableCloudMessage,
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = localhost,
                NetworkManagmentSystemListeningPort = 6666,
                SignallingCloudListeningPort = 20000
            });
            networkNode2.UpdateState += (sender, state) => Console.WriteLine("NetworkNode 2.1: " + state);
            networkNode2.Initialize();

            Thread.Sleep(1000);

            var socketNodePortPair1 = new NetworkAddressNodePortPair(clientNodeA.NetworkAddress, portA);
            var socketNodePortPair2 = new NetworkAddressNodePortPair(networkNode1.NetworkAddress, port1);
            var socketNodePortPair3 = new NetworkAddressNodePortPair(networkNode1.NetworkAddress, port2);
            var socketNodePortPair4 = new NetworkAddressNodePortPair(networkNode2.NetworkAddress, port3);
            var socketNodePortPair5 = new NetworkAddressNodePortPair(networkNode2.NetworkAddress, port4);
            var socketNodePortPair6 = new NetworkAddressNodePortPair(clientNodeB.NetworkAddress, portB);

            cableCloud.AddLink(socketNodePortPair1, socketNodePortPair2);
            cableCloud.AddLink(socketNodePortPair3, socketNodePortPair4);
            cableCloud.AddLink(socketNodePortPair5, socketNodePortPair6);

            Thread.Sleep(1000);

            //var sb = new StringBuilder();

            //for (var i = 0; i < 5000; i++) sb.Append("0123456789");

            clientNodeA.Connect("B", 10);

            //Thread.Sleep(1000);

            //clientNodeA.SendMessage("Message", "B");

            Thread.Sleep(1000);
        }

        [TestMethod]
        public void InitializeClientNodeTest() {
            var clientNodePb = new ClientNodeView {
                Parameters = new ClientNodeModel {
                    ClientName = "Janusz",
                    CableCloudListeningPort = 10000,
                    IpAddress = "127.0.0.1"
                }
            };
            clientNodePb.Initialize().Start();
        }

        [TestMethod]
        public void InitializeNetworkNodeTest() {
            var networkNodePictureBox = new NetworkNodeView {
                Parameters = new NetworkNodeModel {
                    NumberOfPorts = 8,
                    CableCloudListeningPort = 10000,
                    IpAddress = "127.0.0.1",
                    NetworkManagmentSystemListeningPort = 6666,
                    SignallingCloudListeningPort = PortRandomizer.RandomFreePort()
                }
            };

            networkNodePictureBox.Initialize().Start();
        }
    }
}