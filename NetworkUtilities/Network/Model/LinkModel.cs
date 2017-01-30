using NetworkUtilities.Utilities;

namespace NetworkUtilities.Network.Model {
    public class LinkModel {
        public NetworkAddressNodePortPair InputNodePortPair { get; set; }
        public NetworkAddressNodePortPair OutputNodePortPair { get; set; }
        public UniqueId BeginNodeViewId { get; set; }
        public UniqueId EndNodeViewId { get; set; }
    }
}