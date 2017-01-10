using NetworkUtilities;

namespace NetworkEmulation.Network.Element {
    public class LinkModel {
        public SocketNodePortPair InputNodePortPair { get; set; }
        public SocketNodePortPair OutputNodePortPair { get; set; }
        public UniqueId BeginNodePictureBoxId { get; set; }
        public UniqueId EndNodePictureBoxId { get; set; }
    }
}