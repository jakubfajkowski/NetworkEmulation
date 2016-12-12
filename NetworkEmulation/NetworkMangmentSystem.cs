using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetworkEmulation
{
    public class NetworkMangmentSystem : LogObject
    {
        private Dictionary<int, DateTime> keepAliveDictionary;
        private List<ConnectionTableRow> connectionTable;
        private UdpClient listenUdpClient;
        private const int listenUdpPort = 6666;



        public NetworkMangmentSystem()
        {
            keepAliveDictionary = new Dictionary<int, DateTime>();
            connectionTable = new List<ConnectionTableRow>();

            start();
        }


        private void start()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, listenUdpPort);
            listenUdpClient = new UdpClient(ipEndPoint);

            listenForConnectionRequests();
        }

        private void listenForConnectionRequests()
        {
            Task.Run(async () => {
                using (listenUdpClient)
                {
                    while (true)
                    {
                        Console.WriteLine("czeka na zgloszenie");
                        var receivedData = await listenUdpClient.ReceiveAsync();
                        Console.WriteLine("Otrzymal " + BitConverter.ToString(receivedData.Buffer));
                        //EstabilishNodeConnection(BitConverter.ToInt32(receivedData.Buffer, 0));
                    }
                }
            });
        }
    }
}