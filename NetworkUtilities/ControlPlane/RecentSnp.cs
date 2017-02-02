using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class RecentSnp {
        public SubnetworkPoint SubnetworkPoint { get; private set; }
        public int Port { get; private set; }

        public RecentSnp(SubnetworkPoint subnetworkPoint, int port) {
            SubnetworkPoint = subnetworkPoint;
            Port = port;
        }
    }
}