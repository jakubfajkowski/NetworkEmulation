using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.Element {
    public class SubnetworkPointPool {
        public NetworkAddress NetworkSnppAddress { get; private set; }
        public int PortSnpp { get; private set; }
        public List<string> Snp { get; set; }

        SubnetworkPointPool(NetworkAddress networkAddress, int port) {
            NetworkSnppAddress = networkAddress.Append(port);
        }
    }
}

