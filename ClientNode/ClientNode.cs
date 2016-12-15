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
    [XmlRoot("ClientNode")]
    public class ClientNode : Node, IXmlSerializable {
        public string message;
        public string receivedMessage;

        [XmlElement("clientName", typeof(string))]
        private string clientName { get; set; }

        [XmlElement("vpi", typeof(int))]
        private int vpi { get; set; }

        [XmlElement("vci", typeof(int))]
        private int vci { get; set; }

        [XmlElement("portNumber", typeof(int))]
        private int portNumber { get; set; }

        List<ClientTableRow> clients;
        MainForm mainForm;

        public ClientNode() : base() {
        }


        public ClientNode(string[] args) : base() {
            string.Join(" ", args);
            clients = new List<ClientTableRow>();
        }

        //------------------------------Do zmiany!-------------------------------------------------
        public void addClient(int vpi, int vci, int portNumber, string clientName) {
            clients.Add(new ClientTableRow(vpi, vci, portNumber, clientName));
            mainForm.addClientToComboBox(clientName);
        }

        //---------------------------------------------------------------------------------------
        public CableCloudMessage createCableCloudMessage() {
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


        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            var stringSerializer = new XmlSerializer(typeof(string));
            reader.ReadStartElement("ClientNode");
            clientName= (string)stringSerializer.Deserialize(reader);
            var intSerializer = new XmlSerializer(typeof(int));
            vci = (int)intSerializer.Deserialize(reader);
            vpi = (int)intSerializer.Deserialize(reader);
            portNumber = (int)intSerializer.Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            var stringSerializer = new XmlSerializer(typeof(string));
            stringSerializer.Serialize(writer, clientName);
            var intSerializer = new XmlSerializer(typeof(int));
            intSerializer.Serialize(writer,vci);
            intSerializer.Serialize(writer,vpi);
            intSerializer.Serialize(writer,portNumber);
        }
    }
}