using NetworkUtilities.element;

namespace NetworkEmulation.network.element {
    public class LinkSerializableParameters : SerializableParameters {
        public SocketNodePortPair InputNodePortPair { get; set; }
        public SocketNodePortPair OutputNodePortPair { get; set; }
        public int BeginNodePictureBoxId { get; set; }
        public int EndNodePictureBoxId { get; set; }
    }
}