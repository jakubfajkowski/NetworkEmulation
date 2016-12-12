using NetworkUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkNode
{
    public class Port
    {
        /* Globalny numer łącza rzeczywistego podłączonego do danego portu */
        int portNumber;
        
        List<ATMCell> portBuffer;
        DateTime lastAddTime;


        public Port(int portNumber)
        {
            this.portNumber = portNumber;
            portBuffer = new List<ATMCell>();
            lastAddTime = DateTime.Now;
        }

        public ATMCell getATMCell()
        {
            if (portBuffer.Count > 0)
            {
                ATMCell cell = portBuffer[0];
                portBuffer.Remove(cell);
                return cell;
            }
            else
                return null;
        }

        public void addATMCell(ATMCell cell)
        {
            portBuffer.Add(cell);
            lastAddTime = DateTime.Now;
            //Console.WriteLine("Dodanie ATMCell do bufora wejsciowego/wyjsciowego, liczba komórek w buforze: " + portBuffer.Count);
        }

        public int getPortNumber()
        {
            return portNumber;
        }

        public DateTime getLastAddTime()
        {
            return lastAddTime;
        }

        public int getATMCellNumber()
        {
            return portBuffer.Count;
        }
    }
}

