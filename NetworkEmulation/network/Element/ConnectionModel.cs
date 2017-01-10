using System.Collections.Generic;
using NetworkUtilities;

namespace NetworkEmulation.Network.Element {
    public class ConnectionModel {
        public List<UniqueId> LinksIds { get; set; } = new List<UniqueId>();

        public List<NodeConnectionInformation> NodeConnectionInformations { get; set; } =
            new List<NodeConnectionInformation>();

        public UniqueId BeginClientNodePictureBoxId { get; set; }
        public UniqueId EndClientNodePictureBoxId { get; set; }
    }
}