using System.Collections.Generic;
using NetworkUtilities;

namespace NetworkEmulation.network.element {
    public class ConnectionSerializableParameters {
        public List<UniqueId> LinksIds { get; set; } = new List<UniqueId>();
        public List<NodeConnectionInformation> NodeConnectionInformations { get; set; } = new List<NodeConnectionInformation>();
    }
}