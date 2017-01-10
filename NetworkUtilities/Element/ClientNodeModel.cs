using System.Collections.Generic;

namespace NetworkUtilities.Element {
    public class ClientNodeModel : NodeModel {
        public string ClientName { get; set; }
        public List<ClientTableRow> ClientTable { get; set; } = new List<ClientTableRow>();
    }
}