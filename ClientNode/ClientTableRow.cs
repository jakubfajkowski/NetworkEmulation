using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ClientNode {
    [XmlRoot("ClientTable")]
    public class ClientTableRow : IXmlSerializable {

        [XmlElement("clientName", typeof(string))]
        private string clientName { get; set; }

        [XmlElement("vpi", typeof(int))]
        private int vpi { get; set; }

        [XmlElement("vci", typeof(int))]
        private int vci { get; set; }

        public ClientTableRow() {}

        public ClientTableRow(int vpi, int vci, string clientName) {
            this.vpi = vpi;
            this.vci = vci;
            this.clientName = clientName;

        }

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            var stringSerializer = new XmlSerializer(typeof(string));
            reader.ReadStartElement("ClientTableRow");
            clientName = (string)stringSerializer.Deserialize(reader);
            var intSerializer = new XmlSerializer(typeof(int));
            vci = (int)intSerializer.Deserialize(reader);
            vpi = (int)intSerializer.Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            var stringSerializer = new XmlSerializer(typeof(string));
            stringSerializer.Serialize(writer, clientName);
            var intSerializer = new XmlSerializer(typeof(int));
            intSerializer.Serialize(writer, vci);
            intSerializer.Serialize(writer, vpi);
        }
    }
}
