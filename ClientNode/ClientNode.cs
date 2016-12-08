using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities;


namespace ClientNode
{
    public class ClientNode
    {
        public string message;
        public string receivedMessage;
        private int numberOfClients;
        private string clientName;
        //lista przechowująca komórki ATM
        //public List<ATMCell> atmCells = new List<ATMCell>();
        List<ClientTableRow> clients;
        MainForm mainForm;

        public ClientNode(MainForm mainForm) {
            this.mainForm = mainForm;

            clients = new List<ClientTableRow>();
        }

        public ClientNode(string clientName) {
            this.clientName = clientName;

            clients = new List<ClientTableRow>();
            numberOfClients++;
        }

        public void addClient(int vpi, int vci, int linkNumber, string clientName) {
            clients.Add(new ClientTableRow(vpi, vci, linkNumber, clientName));
            mainForm.addClientToComboBox(clientName);
        }

        //metoda zamieniająca tekst na bity i dzieląca je na komórki ATM
        public void createATMCell(int vpi, int vci, string message, List<ATMCell> atmCells)
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


        public void receiveCableCloudMessage(CableCloudMessage message) {
            StringBuilder sb = new StringBuilder();
            foreach (ATMCell cell in message.atmCells) {
                sb.Append(Encoding.UTF8.GetString(cell.data));
            }
            receivedMessage = sb.ToString();
        }


    }
}
