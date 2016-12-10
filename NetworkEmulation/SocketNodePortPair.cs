using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NetworkEmulation {
    [XmlRoot("SocketNodePortPair")]
    public class SocketNodePortPair : IXmlSerializable {
        [XmlElement("NodePortNumber", typeof(int))]
        public int NodePortNumber { get; private set; }
        [XmlElement("SocketPortNumber", typeof(int))]
        public int SocketPortNumber { get; private set; }

        private SocketNodePortPair() { }

        public SocketNodePortPair(int nodePortNumber, int socketPortNumber) {
            NodePortNumber = nodePortNumber;
            SocketPortNumber = socketPortNumber;
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

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            XmlSerializer intSerializer = new XmlSerializer(typeof(int));

            reader.ReadStartElement("SocketNodePortPair");
            NodePortNumber = (int)intSerializer.Deserialize(reader);
            SocketPortNumber = (int)intSerializer.Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            XmlSerializer intSerializer = new XmlSerializer(typeof(int));

            intSerializer.Serialize(writer, NodePortNumber);
            intSerializer.Serialize(writer, SocketPortNumber);
        }
    }
}
