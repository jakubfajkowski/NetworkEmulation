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

            NetworkNode networkNode = new NetworkNode();
            addConnection(networkNode);
            Test1(networkNode);


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



            Console.In.ReadLine();
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

            /*   Console.WriteLine("Dodawanie komorki ATM");
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
               Thread.Sleep(500);
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 23);
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 23);
               Thread.Sleep(500);
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 11);
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 31);
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 23);
                 Thread.Sleep(3000);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 31);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 11);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 Thread.Sleep(500);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 23);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 23);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 23);
                 Thread.Sleep(500);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 Thread.Sleep(5000);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 23);
                 Thread.Sleep(500);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 Thread.Sleep(500);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 Thread.Sleep(500);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 23);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 23);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 31);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 31);
               */
/*
            CableCloudMessage message = new CableCloudMessage(23);
            for (int i=0;i<32;i++)
            {
                message.add(new ATMCell(4, 42, null));
                message.add(new ATMCell(3, 90, null));
                message.add(new ATMCell(4, 42, null));
                message.add(new ATMCell(1, 2, null));
            }
            networkNode.receiveCableCloudMessage(message);
            message = new CableCloudMessage(11);
            Thread.Sleep(10000);
            for (int i = 0; i < 32; i++)
            {
                message.add(new ATMCell(4, 42, null));
                message.add(new ATMCell(3, 90, null));
                message.add(new ATMCell(4, 42, null));
                message.add(new ATMCell(1, 2, null));
            }
            networkNode.receiveCableCloudMessage(message);
            */
              
            //for (int i=0; i<100;i++)
            //networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null));

        }



    }
}

/*
TU DU:
- W NetworkNode zmienić pola z public na private
*/
