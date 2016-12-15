using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities;
using System.Threading;
using System.Runtime.CompilerServices;

namespace NetworkNode
{
    public class CommutationMatrix
    {
        // Tablica połączeń in/out ta sama, która się znajduje w NetworkNodeAgent
        private CommutationTable commutationTable;
        //private List<ATMCell> matrixBuffer;
        /* Obiekt potrzebny do wywołania metody addATMCellToOutBuffer() klasy NetworkNode*/
        private NetworkNode networkNode;

        private List<Port> inputPorts;
        private List<Port> outputPorts;

        private bool timeToQuit = false;
        private Thread matrixThread;

        private bool commuted;


        public CommutationMatrix(CommutationTable comTable, NetworkNode networkNode)
        {
            this.networkNode = networkNode;
            commutationTable = comTable;
            //matrixBuffer = new List<ATMCell>();
            inputPorts = new List<Port>();
            outputPorts = new List<Port>();

            matrixThread = new Thread(runThread);
            matrixThread.Start();
        }


        private void runThread()
        {
            int j = 0;
            while (!timeToQuit)
            {
                //Console.WriteLine("Wywolanie run matrix: " + j++);
                commuted = false;

                foreach (Port inPort in inputPorts)
                {
                    //Console.WriteLine("Wchodzi");     
                    ATMCell cell = inPort.getATMCell();
                    if (cell != null)
                    {
                        commute(cell, inPort.getPortNumber());
                        commuted = true;
                    }    
                }
                if (!commuted)
                {                 
                    lock (matrixThread)
                    {
                        Monitor.Wait(matrixThread);
                    }
                    //Console.WriteLine("wątek run matrix wstał");
                }
            }
        }


        /* Metoda dodająca komórkę ATM do bufora pola komutacyjnego*/
        public void addATMCellToInputPort(ATMCell cell, int portNumber)
        {
            //Console.WriteLine("Wyszukiwanie portu wejściowego...");
            foreach (Port inPort in inputPorts)
            {
                if (portNumber == inPort.getPortNumber())
                {
                    inPort.addATMCell(cell);
                    //Console.WriteLine("Dodanie komórki ATM do portu o łączu wejściowym " + inPort.getLinkNumber());

                    lock (matrixThread)
                    {
                        Monitor.Pulse(matrixThread);
                    }
                    
                    return;
                }
            }    
        }

       
        /* Metoda zmieniająca VPI, VCI na podstawie tabeli */
        public bool commute(ATMCell cell, int inPortNumber)
        {
            //Console.WriteLine("tu wchodzi");
            CommutationTableRow row = commutationTable.check(cell.VPI, cell.VCI, inPortNumber);
            if (row != null)
            {
                cell.VPI = row.getOutVPI();
                if (row.getOutVCI() != -1)
                    cell.VCI = row.getOutVCI();

                Console.WriteLine("Zmiana VPI/VCI na " + cell.VPI + "/" + cell.VCI + " Wrzucenie komórki do portu wyjściowego o łączu " + row.getOutPort());
                return addATMCellToOutputPort(cell, row.getOutPort());
            }
            return false;
        }

        private bool addATMCellToOutputPort(ATMCell cell, int portNumber)
        {
            foreach (Port outPort in outputPorts)
            {
                if (portNumber == outPort.getPortNumber())
                {
                    outPort.addATMCell(cell);
                    return true;
                }
            }
            return false;
        }

        public List<Port> getOutputPortList()
        {
            return outputPorts;
        }


        public bool createInputPort(int portNumber)
        {
            bool isFree = true;
            foreach (Port port in inputPorts)
            {
                if (port.getPortNumber() == portNumber)
                    isFree = false;
            }
            if (isFree)
            {
                inputPorts.Add(new Port(portNumber));
                Console.WriteLine("Udalo sie utworzyc port wejsciowy " + portNumber);
            }
            else
                Console.WriteLine("Nie udalo sie utworzyc portu");
            return isFree;
        }

        public bool createOutputPort(int portNumber)
        {
            bool isFree = true;
            foreach (Port port in outputPorts)
            {
                if (port.getPortNumber() == portNumber)
                    isFree = false;
            }
            if (isFree)
            {
                outputPorts.Add(new Port(portNumber));
                Console.WriteLine("Udalo sie utworzyc port wyjsciowy " + portNumber);
            }
            else
                Console.WriteLine("Nie udalo sie utworzyc portu");
            return isFree;              
        }


        
    }
}
