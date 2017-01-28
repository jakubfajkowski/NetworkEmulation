using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    class HierarchicalPathComputationServer : PathComputationServer {
        private readonly ConnectionController _connectionController;
        private readonly RoutingController _routingController;

        public HierarchicalPathComputationServer(NetworkAddress networkAddress, int port) : base(networkAddress, port) {
            _connectionController = new ConnectionController(networkAddress);
            _routingController = new RoutingController(networkAddress);
        }
    }
}
