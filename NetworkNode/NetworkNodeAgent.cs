using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkNode
{
    public class NetworkNodeAgent
    {
        private const int nmsPort = 6666;

        private CommutationTable commutationTable;
        private CommutationMatrix commutationMatrix;

        private UdpClient udpClient;
        private IPEndPoint ipEndpoint;
        private const int sleepTimeKeepAlive = 500;

        private UdpClient listenUdpClient;
        public int listenUdpPort;


        public NetworkNodeAgent(int port)
        {          
            listenUdpPort = port;

            commutationTable = new CommutationTable();

            var ipEndPoint = new IPEndPoint(IPAddress.Any, listenUdpPort);
            listenUdpClient = new UdpClient(ipEndPoint);

            listenForConnectionRequests();       

            connectToNMS();
        }

        private void connectToNMS()
        {
            udpClient = new UdpClient();
            ipEndpoint = new IPEndPoint(IPAddress.Loopback, nmsPort);
            // Wiadomość, że węzeł wstał
            sendToNMS(Encoding.UTF8.GetBytes("networkNodeStart " + listenUdpPort));

            Thread keepAliveThread = new Thread(keepAliveThreadRun);
            keepAliveThread.Start();
        }

        private void sendToNMS(byte[] bytesToSend)
        {
            udpClient.Send(bytesToSend, bytesToSend.Length, ipEndpoint);
        }

        private void sendToNMS(byte[] bytesToSend, int port)
        {
            udpClient.Send(bytesToSend, bytesToSend.Length, new IPEndPoint(IPAddress.Loopback, port));
        }

        private void keepAliveThreadRun()
        {
            byte[] keepAliveMessage = Encoding.UTF8.GetBytes("keepAlive " + listenUdpPort);
            while (true)
            {
                sendToNMS(keepAliveMessage);
                Thread.Sleep(sleepTimeKeepAlive);
            }
        }

        private void listenForConnectionRequests()
        {
            Task.Run(async () => {
                using (listenUdpClient)
                {
                    while (true)
                    {
                        //Console.WriteLine("czeka na zgloszenie od nms (w networkNode) "+ listenUdpPort);
                        var receivedData = await listenUdpClient.ReceiveAsync();

                        string message = Encoding.UTF8.GetString(receivedData.Buffer);
                        Console.WriteLine("Odebral "+message);
                        string[] messageSplit = message.Split(' ');

                        switch (messageSplit[0])
                        {
                            case "CreateConnection":
                                addConnectionToTable(int.Parse(messageSplit[1]), int.Parse(messageSplit[2]), int.Parse(messageSplit[3]),
                                    int.Parse(messageSplit[4]), int.Parse(messageSplit[5]), int.Parse(messageSplit[6]));
                                break;
                            case "CreatePortOut":
                                commutationMatrix.createOutputPort(int.Parse(messageSplit[1]));
                                break;
                            case "CreatePortIn":
                                commutationMatrix.createInputPort(int.Parse(messageSplit[1]));
                                break;
                        }
                    }
                }
            });
        }



        /* Wywołuje metodę tabeli połączeń, która dodaje połączenie */
        public void addConnectionToTable(int inVPI, int inVCI, int inPortNumber,int outVPI, int outVCI, int linkNumber)
        {
            commutationTable.addConnection(inVPI, inVCI, inPortNumber, outVPI, outVCI, linkNumber);
        }

        /* Wywołuje metodę tabeli połączeń, która usuwa połączenie */
        public bool removeConnectionFromTable(int inVPI, int inVCI, int inPortNumber, int outVPI, int outVCI, int outPortNumber)
        {
            return commutationTable.removeConnection(inVPI, inVCI, inPortNumber, outVPI, outVCI, outPortNumber);
        }

        /* Getter potrzebny do tego, żeby przekazać obiekt do pola komutacyjnego (CommutationMatrix) */
        public CommutationTable getCommutationTable()
        {
            return commutationTable;
        }

        public void setCommutationMatrix(CommutationMatrix cm)
        {
            commutationMatrix = cm;
        }
    }
}
