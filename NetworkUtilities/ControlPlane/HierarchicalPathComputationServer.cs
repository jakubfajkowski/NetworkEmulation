using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    class HierarchicalPathComputationServer : ConnectionManager {
        protected override Task Listen(TcpClient nodeTcpClient, int port) {
            throw new NotImplementedException();
        }
    }
}
