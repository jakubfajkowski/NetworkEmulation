using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.Element {
    public class SubnetworkPointPool {
        public string IdNode { get; private set; }
        public int PortSnpp { get; private set; }
        public List<string> Snp { get; set; }

        SubnetworkPointPool(string idNode, int port) {
            IdNode = idNode;
            PortSnpp = port;
        }

        public string GetId() {
            return IdNode + PortSnpp;
        }
    }
}