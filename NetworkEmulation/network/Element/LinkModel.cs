using NetworkUtilities;

namespace NetworkEmulation.Network.Element {
    public class LinkModel {
        public SocketNodePortPair InputNodePortPair { get; set; }
        public SocketNodePortPair OutputNodePortPair { get; set; }
        public UniqueId BeginNodeViewId { get; set; }
        public UniqueId EndNodeViewId { get; set; }
    }
}