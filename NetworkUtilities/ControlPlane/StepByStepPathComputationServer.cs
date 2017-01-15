using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    class StepByStepPathComputationServer : PathComputationServer {
        private readonly ConnectionController _connectionController;
        private readonly NetworkCallController _networkCallController;
        private readonly RoutingController _routingController;

        public StepByStepPathComputationServer(int port) : base(port) {
            _connectionController = new ConnectionController();
            _networkCallController = new NetworkCallController();
            _routingController = new RoutingController();
        }
    }
}
