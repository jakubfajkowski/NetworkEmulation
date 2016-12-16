using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.network;

namespace NetworkEmulationTest {
    [TestClass]
    public class NetworkTest {
        [TestMethod]
        public void CreateBasicNetworkTest() {
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

            var clientNodeA = new ClientNode.ClientNode() {
                ClientName = "A"
            };
            clientNodeA.OnMessageRecieved += (sender, state) => Console.WriteLine(state);
            clientNodeA.OnUpdateState += (sender, state) => Console.WriteLine(state);

            var clientNodeB = new ClientNode.ClientNode() {
                ClientName = "B"
            };
            clientNodeB.OnMessageRecieved += (sender, state) => Console.WriteLine(state);
            clientNodeB.OnUpdateState += (sender, state) => Console.WriteLine(state);

            var networkNode1 = new NetworkNode.NetworkNode();
            var networkNode2 = new NetworkNode.NetworkNode();

            Thread.Sleep(1000);

            networkNode1.CommutationMatrix.CreateInputPort(port1);
            networkNode1.CommutationMatrix.CreateOutputPort(port2);

            networkNode2.CommutationMatrix.CreateInputPort(port3);
            networkNode2.CommutationMatrix.CreateOutputPort(port4);

            var socketNodePortPair1 = new SocketNodePortPair(portA, clientNodeA.CableCloudTcpPort);
            var socketNodePortPair2 = new SocketNodePortPair(port1, networkNode1.CableCloudTcpPort);
            var socketNodePortPair3 = new SocketNodePortPair(port2, networkNode1.CableCloudTcpPort);
            var socketNodePortPair4 = new SocketNodePortPair(port3, networkNode2.CableCloudTcpPort);
            var socketNodePortPair5 = new SocketNodePortPair(port4, networkNode2.CableCloudTcpPort);
            var socketNodePortPair6 = new SocketNodePortPair(portB, clientNodeB.CableCloudTcpPort);

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
    }
}
