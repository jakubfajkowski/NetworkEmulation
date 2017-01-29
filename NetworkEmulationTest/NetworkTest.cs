using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.Editor.Element;
using NetworkEmulation.Network;
using NetworkUtilities;
using NetworkUtilities.Element;

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
            cableCloud.UpdateState += (sender, state) => Console.WriteLine(state);
            cableCloud.StartListening();

            var nms = new NetworkManagmentSystem();
            nms.UpdateState += (sender, state) => Console.WriteLine(state);

            Thread.Sleep(1000);

            var clientNodeA = new ClientNode.ClientNode(new ClientNodeModel {
                NetworkAddress = new NetworkAddress("1.1"),
                MaxAtmCellsNumberInCableCloudMessage = maxAtmCellsInCableCloudMessage,
                ClientName = "A",
                CableCloudListeningPort = 10000,
                IpAddress = localhost
            });
            //clientNodeA.OnMessageReceived += (sender, state) => Console.WriteLine(state);
            clientNodeA.UpdateState += (sender, state) => Console.WriteLine(state);

            var clientNodeB = new ClientNode.ClientNode(new ClientNodeModel {
                NetworkAddress = new NetworkAddress("2.2"),
                MaxAtmCellsNumberInCableCloudMessage = maxAtmCellsInCableCloudMessage,
                ClientName = "B",
                CableCloudListeningPort = 10000,
                IpAddress = localhost
            });
            //clientNodeB.OnMessageReceived += (sender, state) => Console.WriteLine(state);
            clientNodeB.UpdateState += (sender, state) => Console.WriteLine(state);

            var networkNode1 = new NetworkNode.NetworkNode(new NetworkNodeModel {
                NetworkAddress = new NetworkAddress("1.2"),
                MaxAtmCellsNumberInCableCloudMessage = maxAtmCellsInCableCloudMessage,
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = localhost,
                NetworkManagmentSystemListeningPort = 6666,
                NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
            });

            var networkNode2 = new NetworkNode.NetworkNode(new NetworkNodeModel {
                NetworkAddress = new NetworkAddress("2.1"),
                MaxAtmCellsNumberInCableCloudMessage = maxAtmCellsInCableCloudMessage,
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = localhost,
                NetworkManagmentSystemListeningPort = 6666,
                NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
            });

            Thread.Sleep(5000);

            var socketNodePortPair1 = new NetworkAddressNodePortPair(clientNodeA.NetworkAddress, portA);
            var socketNodePortPair2 = new NetworkAddressNodePortPair(networkNode1.NetworkAddress, port1);
            var socketNodePortPair3 = new NetworkAddressNodePortPair(networkNode1.NetworkAddress, port2);
            var socketNodePortPair4 = new NetworkAddressNodePortPair(networkNode2.NetworkAddress, port3);
            var socketNodePortPair5 = new NetworkAddressNodePortPair(networkNode2.NetworkAddress, port4);
            var socketNodePortPair6 = new NetworkAddressNodePortPair(clientNodeB.NetworkAddress, portB);

            cableCloud.AddLink(socketNodePortPair1, socketNodePortPair2);
            cableCloud.AddLink(socketNodePortPair3, socketNodePortPair4);
            cableCloud.AddLink(socketNodePortPair5, socketNodePortPair6);

            nms.SendConnectionToNetworkNodeAgent(networkNode1.NetworkNodeAgent.ListenUdpPort, 1, 1, port1, 2, 2, port2);
            nms.SendConnectionToNetworkNodeAgent(networkNode2.NetworkNodeAgent.ListenUdpPort, 2, 2, port3, 3, 3, port4);

            Thread.Sleep(1000);

            //var sb = new StringBuilder();

            //for (var i = 0; i < 5000; i++) sb.Append("0123456789");

            clientNodeA.SendMessage("Message", "B");

            Thread.Sleep(1000);
        }

        [TestMethod]
        public void InitializeClientNodeTest() {
            var clientNodePB = new NetworkEmulation.Editor.Element.ClientNodeView {
                Parameters = new ClientNodeModel {
                    ClientName = "Janusz",
                    CableCloudListeningPort = 10000,
                    IpAddress = "127.0.0.1"
                }
            };
            clientNodePB.Initialize().Start();
        }

        [TestMethod]
        public void InitializeNetworkNodeTest() {
            var networkNodePictureBox = new NetworkEmulation.Editor.Element.NetworkNodeView {
                Parameters = new NetworkNodeModel {
                    NumberOfPorts = 8,
                    CableCloudListeningPort = 10000,
                    IpAddress = "127.0.0.1",
                    NetworkManagmentSystemListeningPort = 6666,
                    NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
                }
            };

            networkNodePictureBox.Initialize().Start();
        }
    }
}