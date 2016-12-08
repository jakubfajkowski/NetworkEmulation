using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNode {
    class ClientTableRow {
        int VPI { get; }
        int VCI { get; }
        int linkNumber { get; }
        string clientName { get; }

        public ClientTableRow(int vpi, int vci, int linkNumber, string clientName) {
            this.VPI = vpi;
            this.VCI = vci;
            this.linkNumber = linkNumber;
            this.clientName = clientName;

        }

    }
}
