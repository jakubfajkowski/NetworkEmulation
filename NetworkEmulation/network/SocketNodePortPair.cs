using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NetworkEmulation.network {
    [XmlRoot("SocketNodePortPair")]
    public class SocketNodePortPair : IXmlSerializable {
        private SocketNodePortPair() {
        }

        public SocketNodePortPair(int nodePortNumber, int socketPortNumber) {
            NodePortNumber = nodePortNumber;
            SocketPortNumber = socketPortNumber;
        }

        [XmlElement("NodePortNumber", typeof(int))]
        public int NodePortNumber { get; set; }

        [XmlElement("SocketPortNumber", typeof(int))]
        public int SocketPortNumber { get; private set; }

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            var intSerializer = new XmlSerializer(typeof(int));

            reader.ReadStartElement("SocketNodePortPair");
            NodePortNumber = (int) intSerializer.Deserialize(reader);
            SocketPortNumber = (int) intSerializer.Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            var intSerializer = new XmlSerializer(typeof(int));

            intSerializer.Serialize(writer, NodePortNumber);
            intSerializer.Serialize(writer, SocketPortNumber);
        }

        public override bool Equals(object obj) {
            var other = obj as SocketNodePortPair;
            if (other == null)
                return false;

            var nodePortNumberIsEqual = NodePortNumber == other.NodePortNumber;
            var socketPortNumberIsEqual = SocketPortNumber == other.SocketPortNumber;

            return nodePortNumberIsEqual && socketPortNumberIsEqual;
        }

        public override int GetHashCode() {
            return NodePortNumber ^ SocketPortNumber;
        }
    }
}