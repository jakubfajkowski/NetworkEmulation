using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetworkEmulation.log;
using System.Text;

namespace NetworkEmulation.network
{
    public class NetworkMangmentSystem : LogObject
    {
        private Dictionary<int, DateTime> keepAliveDictionary;
        private List<ConnectionTableRow> connectionTable;
        private UdpClient listenUdpClient;
        private const int listenUdpPort = 6666;
       
        private List<string> receivedMessagesList;
        private List<string> receivedConnectionLabelsList;
        private Thread messageThread;


        public NetworkMangmentSystem()
        {
            keepAliveDictionary = new Dictionary<int, DateTime>();
            connectionTable = new List<ConnectionTableRow>();

            receivedMessagesList = new List<string>();
            receivedConnectionLabelsList = new List<string>();

            var ipEndPoint = new IPEndPoint(IPAddress.Any, listenUdpPort);
            listenUdpClient = new UdpClient(ipEndPoint);

            listenForConnectionRequests();

            messageThread = new Thread(runThread);
            messageThread.Start();
        }
        
        

        public ConnectionTableRow getRow(int node1, int node2)
        {
            foreach (ConnectionTableRow row in connectionTable)
            {
                if (row.checkNodes(node1, node2))
                    return row;
            }
            return null;
        }

        // Wpis w tablicy pola komutacyjnego
        public void sendConnectionToNetworkNodeAgent(int nodeUdpPort, int inVPI, int inVCI, int inPortNumber, int outVPI, int outVCI, int outPortNumber)
        {
            sendMessageToNetworkNode("CreateConnection "+ inVPI + " " + inVCI + " " + inPortNumber + " " + outVPI + " " + outVCI + " " + outPortNumber, nodeUdpPort);
        }

        // Utworzenie portów do pola komutacyjnego 
        public void createLink(int node1, int node1OutPortNumber, int node2, int node2InPortNumber)
        {
            sendMessageToNetworkNode("CreatePortOut " + node1OutPortNumber, node1);
            sendMessageToNetworkNode("CreatePortIn " + node2InPortNumber, node2);
        }

        

        /* Wątek obsługujący keep alive*/
        private void runThread()
        {
            while(true)
            {
                if (receivedMessagesList.Count > 0)
                {
                    string[] message = receivedMessagesList[0].Split(' ');

                    switch(message[0])
                    {
                        case "networkNodeStart":
                            keepAliveDictionary.Add(int.Parse(message[1]), DateTime.Now);
                            //sendMessageToNetworkNode(Encoding.UTF8.GetBytes("OK " + int.Parse(message[1])), int.Parse(message[1]));
                            break;
                        case "keepAlive":
                            //Console.WriteLine(receivedMessagesList[0]);
                            keepAliveDictionary[int.Parse(message[1])] = DateTime.Now;
                            break;                 
                    }
                    receivedMessagesList.Remove(receivedMessagesList[0]); 
                }
                else
                {
                    lock (messageThread)
                    {
                        Monitor.Wait(messageThread);
                    }
                }
            }
        }

        private void addToMessageList(string message)
        {
            receivedMessagesList.Add(message);
            lock (messageThread)
            {
                Monitor.Pulse(messageThread);
            }
        }


        private void sendMessageToNetworkNode(string message, int port)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            var udpClient = new UdpClient();
            var ipEndpoint = new IPEndPoint(IPAddress.Loopback, port);
            udpClient.Send(bytes, bytes.Length, ipEndpoint);
        }

        private void listenForConnectionRequests()
        {
            Task.Run(async () => {
                using (listenUdpClient)
                {
                    while (true)
                    {
                        //Console.WriteLine("czeka na zgloszenie");
                        var receivedData = await listenUdpClient.ReceiveAsync();
                        //Console.WriteLine("Otrzymal " + System.Text.Encoding.UTF8.GetString(receivedData.Buffer));
                        string message = Encoding.UTF8.GetString(receivedData.Buffer);
                        addToMessageList(message);
                        //EstabilishNodeConnection(BitConverter.ToInt32(receivedData.Buffer, 0));
                    }
                }
            });
        }
    }
}