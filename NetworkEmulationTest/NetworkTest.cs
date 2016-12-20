using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.editor.element;
using NetworkEmulation.network;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkEmulationTest {
    [TestClass]
    public class NetworkTest {
        [TestMethod]
        public void CreateBasicNetworkTest() {
            var localhost = "127.0.0.1";

            var portA = 1;
            var port1 = 2;
            var port2 = 3;
            var port3 = 4;
            var port4 = 5;
            var portB = 6;

            var cableCloud = new CableCloud();
            cableCloud.OnUpdateState += (sender, state) => Console.WriteLine(state);
            var nms = new NetworkManagmentSystem();
            nms.OnUpdateState += (sender, state) => Console.WriteLine(state);

            Thread.Sleep(1000);

            var clientNodeA = new ClientNode.ClientNode(new ClientNodeSerializableParameters {
                ClientName = "A",
                ClientTable = new List<ClientTableRow>(new []{new ClientTableRow("B", portA, 1, 1)}),
                CableCloudListeningPort = 10000,
                IpAddress = localhost,
                CableCloudDataPort = PortRandomizer.RandomFreePort()
            });
            clientNodeA.OnMessageRecieved += (sender, state) => Console.WriteLine(state);
            clientNodeA.OnUpdateState += (sender, state) => Console.WriteLine(state);

            var clientNodeB = new ClientNode.ClientNode(new ClientNodeSerializableParameters {
                ClientName = "B",
                CableCloudListeningPort = 10000,
                IpAddress = localhost,
                CableCloudDataPort = PortRandomizer.RandomFreePort()
            });
            clientNodeB.OnMessageRecieved += (sender, state) => Console.WriteLine(state);
            clientNodeB.OnUpdateState += (sender, state) => Console.WriteLine(state);

            var networkNode1 = new NetworkNode.NetworkNode(new NetworkNodeSerializableParameters {
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = localhost,
                CableCloudDataPort = PortRandomizer.RandomFreePort(),
                NetworkManagmentSystemListeningPort = 6666,
                NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
            });
            var networkNode2 = new NetworkNode.NetworkNode(new NetworkNodeSerializableParameters{
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = localhost,
                CableCloudDataPort = PortRandomizer.RandomFreePort(),
                NetworkManagmentSystemListeningPort = 6666,
                NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
            });

            Thread.Sleep(5000);

            var socketNodePortPair1 = new SocketNodePortPair(portA, clientNodeA.CableCloudDataPort);
            var socketNodePortPair2 = new SocketNodePortPair(port1, networkNode1.CableCloudDataPort);
            var socketNodePortPair3 = new SocketNodePortPair(port2, networkNode1.CableCloudDataPort);
            var socketNodePortPair4 = new SocketNodePortPair(port3, networkNode2.CableCloudDataPort);
            var socketNodePortPair5 = new SocketNodePortPair(port4, networkNode2.CableCloudDataPort);
            var socketNodePortPair6 = new SocketNodePortPair(portB, clientNodeB.CableCloudDataPort);

            cableCloud.AddLink(socketNodePortPair1, socketNodePortPair2);
            cableCloud.AddLink(socketNodePortPair3, socketNodePortPair4);
            cableCloud.AddLink(socketNodePortPair5, socketNodePortPair6);

            nms.SendConnectionToNetworkNodeAgent(networkNode1.NetworkNodeAgent.ListenUdpPort, 1, 1, port1, 2, 2, port2);
            nms.SendConnectionToNetworkNodeAgent(networkNode2.NetworkNodeAgent.ListenUdpPort, 2, 2, port3, 3, 3, port4);

            Thread.Sleep(1000);

            clientNodeA.AddClient("B", portA, 1, 1);

            clientNodeA.SendMessage("Lorem ipsum...", "B");

            Thread.Sleep(1000);
        }

        [TestMethod]
        public void InitializeClientNodeTest() {
            ClientNodePictureBox clientNodePB= new ClientNodePictureBox() {
                Parameters = new ClientNodeSerializableParameters {
                    ClientName = "Janusz",
                    ClientTable =
                        new List<ClientTableRow>(new[]
                            {new ClientTableRow("clientName", 1, 2, 3), new ClientTableRow("clientName2", 1, 2, 3)}),
                    CableCloudListeningPort = 10000,
                    IpAddress = "127.0.0.1"
                }
            };
            clientNodePB.Initialize().Start();
        }

        [TestMethod]
        public void InitializeNetworkNodeTest() {
            var networkNodePictureBox = new NetworkNodePictureBox {
                Parameters = new NetworkNodeSerializableParameters {
                    NumberOfPorts = 8,
                    CableCloudListeningPort = 10000,
                    IpAddress = "127.0.0.1",
                    CableCloudDataPort = PortRandomizer.RandomFreePort(),
                    NetworkManagmentSystemListeningPort = 6666,
                    NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
                }
            };

            networkNodePictureBox.Initialize().Start();
        }
    }
}