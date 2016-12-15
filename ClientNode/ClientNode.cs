using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities;
using System.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ClientNode {
    //[XmlRoot("ClientNode")]
    public class ClientNode : Node {
        public string message;
        public string receivedMessage;

        public string ClientName { get; set; }

        private int portNumber = 10000;

        public List<ClientTableRow> Clients { get; set; }

        public ClientNode() : base() {
            Clients = new List<ClientTableRow>();
        }


        public ClientNode(string[] args) : base() {
            string.Join(" ", args);
            Clients = new List<ClientTableRow>();
        }

        public void addClient(int vpi, int vci, string clientName) {
            Clients.Add(new ClientTableRow(vpi, vci, clientName));
            //mainForm.addClientToComboBox(clientName);
        }

        public CableCloudMessage createCableCloudMessage(int vpi, int vci) {
            CableCloudMessage cableCloudMessage = new CableCloudMessage(portNumber);

            byte[] source = Encoding.UTF8.GetBytes(message);

            for (int i = 0; i < source.Length; i += 48) {
                byte[] buffer = new byte[48];
                if (i <= source.Length - 48) {
                    Buffer.BlockCopy(source, i, buffer, 0, 48);
                    cableCloudMessage.add(new ATMCell(vpi, vci, buffer));
                }
                else
                    // gdy długość wiadomości jest mniejsza od 48 bitów, komórka jest wypełniana '0' na pozostałych miejscach
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


        //public XmlSchema GetSchema() {
        //    return null;
        //}

        //public void ReadXml(XmlReader reader) {
        //    var stringSerializer = new XmlSerializer(typeof(string));
        //    var clientsSerializer = new XmlSerializer(typeof(List<ClientTableRow>));

        //    reader.ReadStartElement("ClientNode");
        //    clientName= (string)stringSerializer.Deserialize(reader);

        //    clients = clientsSerializer.Deserialize(reader) as List<ClientTableRow>;
        //    reader.ReadEndElement();
        //}

        //public void WriteXml(XmlWriter writer) {
        //    var stringSerializer = new XmlSerializer(typeof(string));
        //    var clientsSerializer = new XmlSerializer(typeof(List<ClientTableRow>));

        //    stringSerializer.Serialize(writer, clientName);
            
        //    clientsSerializer.Serialize(writer, clients);
        //}
    }
}