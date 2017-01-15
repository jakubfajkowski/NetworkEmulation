using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    class HierarchicalPathComputationServer : PathComputationServer {
        private readonly ConnectionController connectionController;
        private readonly RoutingController routingController;

        public HierarchicalPathComputationServer(int port) : base(port) {
            connectionController = new ConnectionController();
            routingController = new RoutingController();
        }
    }
}
