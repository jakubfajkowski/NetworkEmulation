using System.Collections.Generic;
using NetworkUtilities.element;

namespace NetworkEmulation.network.element {
    public class ConnectionSerializableParameters : SerializableParameters {
        public List<int> LinksIds { get; set; }
    }
}