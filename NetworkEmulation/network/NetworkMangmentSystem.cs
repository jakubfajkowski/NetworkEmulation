using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetworkEmulation.log;

namespace NetworkEmulation.network
{
    public class NetworkMangmentSystem : LogObject
    {
        private Dictionary<int, DateTime> keepAliveDictionary;
        private List<ConnectionTableRow> connectionTable;
        private UdpClient listenUdpClient;
        private const int listenUdpPort = 6666;
        private List<string> receivedMessagesList;
        private Thread messageThread;


        public NetworkMangmentSystem()
        {
            keepAliveDictionary = new Dictionary<int, DateTime>();
            connectionTable = new List<ConnectionTableRow>();

            receivedMessagesList = new List<string>();
            

            var ipEndPoint = new IPEndPoint(IPAddress.Any, listenUdpPort);
            listenUdpClient = new UdpClient(ipEndPoint);

            listenForConnectionRequests();

            messageThread = new Thread(runThread);
            messageThread.Start();
        }

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
                            break;
                        case "keepAlive":                         
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
                        addToMessageList(System.Text.Encoding.UTF8.GetString(receivedData.Buffer));
                        //EstabilishNodeConnection(BitConverter.ToInt32(receivedData.Buffer, 0));
                    }
                }
            });
        }
    }
}