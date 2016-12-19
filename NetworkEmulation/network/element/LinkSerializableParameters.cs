using NetworkUtilities;

namespace NetworkEmulation.network.element {
    public class LinkSerializableParameters {
        public SocketNodePortPair InputNodePortPair { get; set; }
        public SocketNodePortPair OutputNodePortPair { get; set; }
        public UniqueId BeginNodePictureBoxId { get; set; }
        public UniqueId EndNodePictureBoxId { get; set; }
    }
}