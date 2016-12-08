using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities;
using System.Threading;

namespace NetworkNode
{
    
    class NetworkNode : Node
    {
        public CommutationMatrix commutationMatrix;
        public NetworkNodeAgent networkNodeAgent;

        private bool timeToQuit = false;
        private Thread networkNodeThread;

        private List<Port> outputCommutationMatrixPorts;
        private bool sent;

        /* Minimalny czas po jakim komórki ATM zostaną wysłane z portu wyjściowego pola komutacyjnego [ms]*/
        private const int minLastAddTime = 5000;
        private const int maxATMCellNumberInCableCloudMessage = 20;
        private const int sleepTime = 500;
        

        public NetworkNode(int portA, int portC) : base(portA,portC)
        {
            networkNodeAgent = new NetworkNodeAgent();
            commutationMatrix = new CommutationMatrix(networkNodeAgent.getCommutationTable(), this);
            outputCommutationMatrixPorts = commutationMatrix.getOutputPortList();

            networkNodeThread = new Thread(runThread);
            networkNodeThread.Start();
        }

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
                        CableCloudMessage message = new CableCloudMessage(port.getLinkNumber());

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

                        Console.WriteLine("Wysyłanie CableCloudMessage na łącze "+ message.linkNumber + " Liczba ATMCell: "+ message.atmCells.Count
                            + " Port: "+ port.getLinkNumber());
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
                commutationMatrix.addATMCellToInputPort(cell, message.linkNumber);
            }
        }

        private void sendCableCloudMessage(CableCloudMessage message)
        {
            // WYSLAC!!!!!!!!!!!!!!!   Wysłać cell
            //Console.WriteLine("VPI/VCI: " + cell.VPI + "/" + cell.VCI);
        }

        protected override void handleMessage(CableCloudMessage message)
        {
            Console.WriteLine("wchodzi do handleMessage");
            //CableCloudMessage message = CableCloudMessage.deserialize(data);
            Console.WriteLine("link number:" + message.linkNumber);
            Console.WriteLine("atm cell: " + message.atmCells.Count);
            receiveCableCloudMessage(message);
        }
    }
}
