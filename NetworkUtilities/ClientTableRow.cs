using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ClientNode {
    public class ClientTableRow {
        public string clientName { get; set; }
        public int vpi { get; set; }
        public int vci { get; set; }

        public ClientTableRow() {}

        public ClientTableRow(int vpi, int vci, string clientName) {
            this.vpi = vpi;
            this.vci = vci;
            this.clientName = clientName;

        }
    }
}
