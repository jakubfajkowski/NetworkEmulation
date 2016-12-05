using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities;


namespace ClientNode
{
    class ClientNode
    {
        public string message;
        public string receivedMessage;
        //lista przechowująca komórki ATM
        public List<ATMCell> atmCells = new List<ATMCell>();

        //metoda zamieniająca tekst na bity i dzieląca je na komórki ATM
        public void createATMCell(int vpi, int vci, string message)
        {
            byte[] source = Encoding.UTF8.GetBytes(message);


            for (int i = 0; i < source.Length; i += 48)
            {
                byte[] buffer = new byte[48];
                if (i <= source.Length - 48)
                {
                    Buffer.BlockCopy(source, i, buffer, 0, 48);
                    atmCells.Add(new ATMCell(vpi, vci, buffer));
                }
                else // gdy długość wiadomości jest mniejsza od 48 bitów, komórka jest wypełniana '0' na pozostałych miejscach
                {
                    Buffer.BlockCopy(source, i, buffer, 0, source.Length - i);
                    atmCells.Add(new ATMCell(vpi, vci, buffer));
                }
            }
        }

        //odczytywanie i interpretowanie informacji charakterystycznej z komórek ATM
        public void readDataFromATMCells()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < atmCells.Count; i++)
            {
                sb.Append(Encoding.UTF8.GetString(atmCells[i].data));
            }
            receivedMessage = sb.ToString();
        }
        

    }
}
