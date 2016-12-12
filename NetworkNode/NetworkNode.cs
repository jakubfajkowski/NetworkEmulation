using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities;
using System.Threading;

namespace NetworkNode
{
    
    public class NetworkNode : Node
    {
        private const int minLastAddTime = 5000;
        private const int maxATMCellNumberInCableCloudMessage = 20;
        private const int sleepTime = 500;
        private const int nmsPort = 6666;

        public int agentPort;
        private TcpListener agentTcpListener;

        public CommutationMatrix commutationMatrix;
        public NetworkNodeAgent networkNodeAgent;

        private bool timeToQuit = false;
        private Thread networkNodeThread;

        private List<Port> outputCommutationMatrixPorts;
        private bool sent;

        private UdpClient udpClient;
        private IPEndPoint ipEndpoint;
        private const int sleepTimeKeepAlive = 500;

        /* Minimalny czas po jakim komórki ATM zostaną wysłane z portu wyjściowego pola komutacyjnego [ms]*/



        public NetworkNode() : base() {
            this.agentPort = freeTcpPort();
            agentTcpListener = createTcpListener(IPAddress.Loopback, agentPort);
            listenForConnectRequest(agentTcpListener);

            networkNodeAgent = new NetworkNodeAgent();
            commutationMatrix = new CommutationMatrix(networkNodeAgent.getCommutationTable(), this);
            outputCommutationMatrixPorts = commutationMatrix.getOutputPortList();

            networkNodeThread = new Thread(runThread);
            networkNodeThread.Start();

            connectToNMS();
        }

        private void connectToNMS()
        {
            udpClient = new UdpClient();
            ipEndpoint = new IPEndPoint(IPAddress.Loopback, nmsPort);
            // Wiadomość, że węzeł wstał
            sendToNMS(Encoding.UTF8.GetBytes("networkNodeStart " + 66));

            Thread keepAliveThread = new Thread(keepAliveThreadRun);
            keepAliveThread.Start();
        }

        private void sendToNMS(byte[] bytesToSend)
        {
            udpClient.Send(bytesToSend, bytesToSend.Length, ipEndpoint);
        }

        private void keepAliveThreadRun()
        {
            byte[] keepAliveMessage = Encoding.UTF8.GetBytes("keepAlive "+ 66);
            while (true)
            {
                sendToNMS(keepAliveMessage);
                Thread.Sleep(sleepTimeKeepAlive);
            }
        }

        /* Wątek pobierający komórki ATM z portów wyjściowych pola komutacyjnego i wysyłający je do chmury kablowej */
        private void runThread()
        {
            int j = 0;
            while (!timeToQuit)
            {
                sent = false;
                if (j<20)
                Console.WriteLine("Wywolanie run outBuffer: " + j++);
                
                foreach (Port port in outputCommutationMatrixPorts)
                {
                    if ((port.getATMCellNumber() != 0) && ((DateTime.Now - port.getLastAddTime()).TotalMilliseconds > minLastAddTime) || (port.getATMCellNumber() > maxATMCellNumberInCableCloudMessage))
                    {
                        Console.WriteLine("Rozpoczęcie tworzenia CableCloudMessage...");
                        CableCloudMessage message = new CableCloudMessage(port.getPortNumber());

                        int ATMCellNumberInPort = port.getATMCellNumber();
                        //Console.WriteLine("Liczba komórek ATM w buforze wyjściowym: " + port.getATMCellNumber());
                        int ATMCellNumberInMessage;
                        if (ATMCellNumberInPort > maxATMCellNumberInCableCloudMessage)
                            ATMCellNumberInMessage = maxATMCellNumberInCableCloudMessage;
                        else
                            ATMCellNumberInMessage = ATMCellNumberInPort;
                        
                        for (int i=0; i<ATMCellNumberInMessage; i++)
                        {
                            message.add(port.getATMCell());                           
                        }

                        Console.WriteLine("Wysyłanie CableCloudMessage na port "+ message.portNumber + " Liczba ATMCell: "+ message.atmCells.Count
                            + " Port: "+ port.getPortNumber());
                        sendCableCloudMessage(message);
                        sent = true;
                    }
                }

                if (!sent)
                {
                    Thread.Sleep(sleepTime);                
                }
            }
        }

        public void receiveCableCloudMessage(CableCloudMessage message)
        {
            foreach(ATMCell cell in message.atmCells)
            {
                commutationMatrix.addATMCellToInputPort(cell, message.portNumber);
            }
        }

        private void sendCableCloudMessage(CableCloudMessage message)
        {
            send(CableCloudMessage.serialize(message));
        }

        /* Metoda wywoływana po wczytaniu danych z wejścia */
        protected override void handleMessage(CableCloudMessage message)
        {
            Console.WriteLine("wchodzi do handleMessage");
            //CableCloudMessage message = CableCloudMessage.deserialize(data);
            //Console.WriteLine("link number:" + message.portNumber);
            //Console.WriteLine("atm cell: " + message.atmCells.Count);
            receiveCableCloudMessage(message);
        }
    }
}
