using System.Xml.Serialization;
using NetworkUtilities;

namespace NetworkEmulation.Network {
    [XmlRoot]
    public class NetworkAddressNodePortPair {
        private NetworkAddressNodePortPair() {
        }

        public NetworkAddressNodePortPair(NetworkAddress networkAddress, int nodePortNumber) {
            NodePortNumber = nodePortNumber;
            NetworkAddress = networkAddress;
        }

        [XmlElement("NodePortNumber", typeof(int))]
        public int NodePortNumber { get; set; }

        [XmlElement("NetworkAddress", typeof(NetworkAddress))]
        public NetworkAddress NetworkAddress { get; set; }

        public override bool Equals(object obj) {
            var other = obj as NetworkAddressNodePortPair;
            if (other == null)
                return false;

            var nodePortNumberIsEqual = NodePortNumber == other.NodePortNumber;
            var networkAddressIsEqual = NetworkAddress.Equals(other.NetworkAddress);

            return nodePortNumberIsEqual && networkAddressIsEqual;
        }

        public override int GetHashCode() {
            return NodePortNumber ^ NetworkAddress.GetHashCode();
        }
    }
}