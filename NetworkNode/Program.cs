using NetworkEmulation.network;
using NetworkUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace NetworkNode {
    class Program {

        static void Main(string[] args) {

            NetworkMangmentSystem nms = new NetworkMangmentSystem();        
            NetworkNode networkNode = new NetworkNode();
            NetworkNode networkNode2 = new NetworkNode();
            NetworkNode networkNode3 = new NetworkNode();
            //addConnection(networkNode);
            //Test1(networkNode);

            

            createConnection(networkNode, networkNode2, networkNode3, nms);

            Console.In.ReadLine();
        }

        static void createConnection(NetworkNode nn1, NetworkNode nn2, NetworkNode nn3, NetworkMangmentSystem nms)
        {
            //nms.createLink(nn1.networkNodeAgent.listenUdpPort, 12, nn2.networkNodeAgent.listenUdpPort, 13);
            //nms.createLink(nn2.networkNodeAgent.listenUdpPort, 24, nn3.networkNodeAgent.listenUdpPort, 44);
            //nms.createLink(nn1.networkNodeAgent.listenUdpPort, 1, nn3.networkNodeAgent.listenUdpPort, 2);

            nn1.commutationMatrix.createInputPort(1);
            nn1.commutationMatrix.createOutputPort(12);

            nms.sendConnectionToNetworkNodeAgent(nn1.networkNodeAgent.listenUdpPort, 11, 34, 1, 29, 56, 12);
            nms.sendConnectionToNetworkNodeAgent(nn1.networkNodeAgent.listenUdpPort, 29, -1, 13, 39, -1, 24);
            nms.sendConnectionToNetworkNodeAgent(nn1.networkNodeAgent.listenUdpPort, 39, 56, 44, 12, 4, 2);

            CableCloudMessage message = new CableCloudMessage(1);
            for (int i = 0; i < 1; i++)
            {
                message.add(new ATMCell(11,34, null));
            }

            Thread.Sleep(1000);
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, nn1.cloudPort);
            
            byte[] data = CableCloudMessage.serialize(message);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            // Thread.Sleep(1000);
            // nms.createLink(nn1.networkNodeAgent.listenUdpPort, nn2.networkNodeAgent.listenUdpPort);

        }

        static void addConnection(NetworkNode networkNode)
        {
            networkNode.networkNodeAgent.addConnectionToTable(1, 2,11,3, 4, 10);
            networkNode.networkNodeAgent.addConnectionToTable(4, 42,11, 33, 2, 13);
            networkNode.networkNodeAgent.addConnectionToTable(3, 90, 11,2, 33, 46);

            networkNode.commutationMatrix.createInputPort(11);
            networkNode.commutationMatrix.createInputPort(23);
            networkNode.commutationMatrix.createInputPort(31);

            networkNode.commutationMatrix.createOutputPort(10);
            networkNode.commutationMatrix.createOutputPort(13);
            networkNode.commutationMatrix.createOutputPort(46);
        }

        static void Test1(NetworkNode networkNode)
        {
            CableCloudMessage message = new CableCloudMessage(11);
            for (int i = 0; i < 8; i++)
            {
                message.add(new ATMCell(4, 42, null));
                message.add(new ATMCell(3, 90, null));
                message.add(new ATMCell(4, 42, null));
                message.add(new ATMCell(1, 2, null));
            }
            /*   byte[] dd = CableCloudMessage.serialize(message);
              Console.WriteLine("wielkosc " + dd.Length);
              CableCloudMessage ddd = CableCloudMessage.deserialize(dd);
              Console.WriteLine("wychodzi");
              Console.WriteLine("link "+ ddd.portNumber);
              Console.WriteLine("wychodzi");

              */

           
            addConnection(networkNode);
          


            //Thread.Sleep(500);

            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, networkNode.cloudPort);
            //Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            byte[] data = CableCloudMessage.serialize(message);
            NetworkStream stream = client.GetStream();
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
