using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities;


namespace ClientNode
{
    public class ClientNode :Node
    {
        public string message;
        public string receivedMessage;
        private int numberOfClients;
        private string clientName;

        List<ClientTableRow> clients;
        MainForm mainForm;

        public ClientNode() :base() {

        }

        public ClientNode(MainForm mainForm) :base(){
            this.mainForm = mainForm;

            clients = new List<ClientTableRow>();
        }

        public ClientNode(string clientName) :base(){
            this.clientName = clientName;

            clients = new List<ClientTableRow>();
            numberOfClients++;
        }

        public void addClient(int vpi, int vci, int linkNumber, string clientName) {
            clients.Add(new ClientTableRow(vpi, vci, linkNumber, clientName));
            mainForm.addClientToComboBox(clientName);
        }

        public CableCloudMessage createCableCloudMessage(int vpi, int vci, string message, int portNumber)
        {
            CableCloudMessage cableCloudMessage = new CableCloudMessage(portNumber);

            byte[] source = Encoding.UTF8.GetBytes(message);
            
            for (int i = 0; i < source.Length; i += 48)
            {
                byte[] buffer = new byte[48];
                if (i <= source.Length - 48)
                {
                    Buffer.BlockCopy(source, i, buffer, 0, 48);
                    cableCloudMessage.add(new ATMCell(vpi, vci, buffer));
                }
                else // gdy długość wiadomości jest mniejsza od 48 bitów, komórka jest wypełniana '0' na pozostałych miejscach
                {
                    Buffer.BlockCopy(source, i, buffer, 0, source.Length - i);
                    cableCloudMessage.add(new ATMCell(vpi, vci, buffer));
                }
            }
            return cableCloudMessage;
        }


        private void sendCableCloudMessage(CableCloudMessage cableCloudMessage) {
            send(CableCloudMessage.serialize(cableCloudMessage));
        }

        private String receiveMessage(CableCloudMessage message) {
            StringBuilder sb = new StringBuilder();
            foreach (ATMCell cell in message.atmCells) {
                sb.Append(Encoding.UTF8.GetString(cell.data));
            }
            return sb.ToString();
        }

        protected override void handleMessage(CableCloudMessage message) {
            receiveMessage(message);
        }
    }
}
