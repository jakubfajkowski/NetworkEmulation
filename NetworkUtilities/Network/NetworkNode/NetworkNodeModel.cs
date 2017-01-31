using NetworkUtilities.Network.Node;

namespace NetworkUtilities.Network.NetworkNode {
    public class NetworkNodeModel : NodeModel {
        public int NumberOfPorts { get; set; }
        public int NetworkManagmentSystemListeningPort { get; set; }
    }
}