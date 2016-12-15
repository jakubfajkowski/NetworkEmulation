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
        // Czas po jakim komórki ATM zostaną spakowane w CCM
        private const int minLastAddTime = 4;
        private const int maxATMCellNumberInCableCloudMessage = 9;
        // Czas usypiania wątku, który tworzy CCM
        private const int sleepTime = 1;
       // private const int nmsPort = 6666;

        public int agentPort;
        private TcpListener agentTcpListener;

        public CommutationMatrix commutationMatrix;
        public NetworkNodeAgent networkNodeAgent;

        private bool timeToQuit = false;
        private Thread networkNodeThread;

        private List<Port> outputCommutationMatrixPorts;
        private bool sent;

      //  private UdpClient udpClient;
      //  private IPEndPoint ipEndpoint;
      //  private const int sleepTimeKeepAlive = 500;

        



        public NetworkNode() : base() {
            this.agentPort = freeTcpPort();
            agentTcpListener = createTcpListener(IPAddress.Loopback, agentPort);
            listenForConnectRequest(agentTcpListener);

            networkNodeAgent = new NetworkNodeAgent(freeTcpPort());
            commutationMatrix = new CommutationMatrix(networkNodeAgent.getCommutationTable(), this);
            outputCommutationMatrixPorts = commutationMatrix.getOutputPortList();
            networkNodeAgent.setCommutationMatrix(commutationMatrix);

            networkNodeThread = new Thread(runThread);
            networkNodeThread.Start();

          
        }

        /* Wątek pobierający komórki ATM z portów wyjściowych pola komutacyjnego i wysyłający je do chmury kablowej */
        private void runThread()
        {
            int j = 0;
            while (!timeToQuit)
            {
                sent = false;
                if (j<0)
                Console.WriteLine(DateTime.Now.Millisecond + "  Wywolanie run outBuffer: " + j++);
                
                foreach (Port port in outputCommutationMatrixPorts)
                {
                    if ((port.getATMCellNumber() != 0) && ((DateTime.Now - port.getLastAddTime()).TotalMilliseconds > minLastAddTime) || (port.getATMCellNumber() > maxATMCellNumberInCableCloudMessage))
                    {
                        //Console.WriteLine("Rozpoczęcie tworzenia CableCloudMessage...");
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

                        Console.WriteLine(DateTime.Now.Millisecond + "  Wysyłanie CableCloudMessage na port "+ message.portNumber + " Liczba ATMCell: "+ message.atmCells.Count
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
        protected override void handleMessage(CableCloudMessage cableCloudMessage)
        {
            receiveCableCloudMessage(cableCloudMessage);
        }
    }
}
