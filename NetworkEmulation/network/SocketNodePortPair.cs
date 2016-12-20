using System.Xml.Serialization;

namespace NetworkEmulation.network {
    [XmlRoot]
    public class SocketNodePortPair {
        private SocketNodePortPair() {
        }

        public SocketNodePortPair(int nodePortNumber, int socketPortNumber) {
            NodePortNumber = nodePortNumber;
            SocketPortNumber = socketPortNumber;
        }

        [XmlElement("NodePortNumber", typeof(int))]
        public int NodePortNumber { get; set; }

        [XmlElement("SocketPortNumber", typeof(int))]
        public int SocketPortNumber { get; set; }

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