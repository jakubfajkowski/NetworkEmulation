using System.Collections.Generic;

namespace NetworkUtilities.element {
    public class ClientNodeSerializableParameters : NodeSerializableParameters {
        public string ClientName { get; set; }
        public List<ClientTableRow> ClientTable { get; set; } = new List<ClientTableRow>();
    }
}