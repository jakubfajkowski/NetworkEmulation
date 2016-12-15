using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkEmulation.network;
using NetworkUtilities;

namespace NetworkNode {
    internal class Program {
        private static void Main(string[] args) {
            var nms = new NetworkMangmentSystem();
            var networkNode = new NetworkNode();
            var networkNode2 = new NetworkNode();
            var networkNode3 = new NetworkNode();
            //addConnection(networkNode);
            //Test1(networkNode);


            CreateConnection(networkNode, networkNode2, networkNode3, nms);

            Console.In.ReadLine();
        }

        private static void CreateConnection(NetworkNode nn1, NetworkNode nn2, NetworkNode nn3,
            NetworkMangmentSystem nms) {
            //nms.createLink(nn1.networkNodeAgent.listenUdpPort, 12, nn2.networkNodeAgent.listenUdpPort, 13);
            //nms.createLink(nn2.networkNodeAgent.listenUdpPort, 24, nn3.networkNodeAgent.listenUdpPort, 44);
            //nms.createLink(nn1.networkNodeAgent.listenUdpPort, 1, nn3.networkNodeAgent.listenUdpPort, 2);

            nn1.CommutationMatrix.CreateInputPort(1);
            nn1.CommutationMatrix.CreateOutputPort(12);

            nms.SendConnectionToNetworkNodeAgent(nn1.NetworkNodeAgent.ListenUdpPort, 11, 34, 1, 29, 56, 12);
            nms.SendConnectionToNetworkNodeAgent(nn1.NetworkNodeAgent.ListenUdpPort, 29, -1, 13, 39, -1, 24);
            nms.SendConnectionToNetworkNodeAgent(nn1.NetworkNodeAgent.ListenUdpPort, 39, 56, 44, 12, 4, 2);

            var message = new CableCloudMessage(1);
            for (var i = 0; i < 1; i++)
                message.Add(new AtmCell(11, 34, null));

            Thread.Sleep(1000);
            var client = new TcpClient();
            client.Connect(IPAddress.Loopback, nn1.CableCloudTcpPort);

            var data = CableCloudMessage.Serialize(message);
            var stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            // Thread.Sleep(1000);
            // nms.createLink(nn1.networkNodeAgent.listenUdpPort, nn2.networkNodeAgent.listenUdpPort);
        }

        private static void AddConnection(NetworkNode networkNode) {
            networkNode.NetworkNodeAgent.AddConnectionToTable(1, 2, 11, 3, 4, 10);
            networkNode.NetworkNodeAgent.AddConnectionToTable(4, 42, 11, 33, 2, 13);
            networkNode.NetworkNodeAgent.AddConnectionToTable(3, 90, 11, 2, 33, 46);

            networkNode.CommutationMatrix.CreateInputPort(11);
            networkNode.CommutationMatrix.CreateInputPort(23);
            networkNode.CommutationMatrix.CreateInputPort(31);

            networkNode.CommutationMatrix.CreateOutputPort(10);
            networkNode.CommutationMatrix.CreateOutputPort(13);
            networkNode.CommutationMatrix.CreateOutputPort(46);
        }

        private static void Test1(NetworkNode networkNode) {
            var message = new CableCloudMessage(11);
            for (var i = 0; i < 8; i++) {
                message.Add(new AtmCell(4, 42, null));
                message.Add(new AtmCell(3, 90, null));
                message.Add(new AtmCell(4, 42, null));
                message.Add(new AtmCell(1, 2, null));
            }
            /*   byte[] dd = CableCloudMessage.serialize(message);
              Console.WriteLine("wielkosc " + dd.Length);
              CableCloudMessage ddd = CableCloudMessage.deserialize(dd);
              Console.WriteLine("wychodzi");
              Console.WriteLine("link "+ ddd.portNumber);
              Console.WriteLine("wychodzi");

              */


            AddConnection(networkNode);


            //Thread.Sleep(500);

            var client = new TcpClient();
            client.Connect(IPAddress.Loopback, networkNode.CableCloudTcpPort);
            //Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            var data = CableCloudMessage.Serialize(message);
            var stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            //Thread.Sleep(10);
            //stream.Write(data, 0, data.Length);
            //Thread.Sleep(10);
            //stream.Write(data, 0, data.Length);
            //Thread.Sleep(10);
            //stream.Write(data, 0, data.Length);  
            //Console.WriteLine("Sent: {0}", message);

            Thread.Sleep(1000);
            stream.Write(data, 0, data.Length);
        }
    }
}

/*
TU DU:
- W NetworkNode zmienić pola z public na private
*/