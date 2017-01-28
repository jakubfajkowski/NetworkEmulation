using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public class StepByStepPathComputationServer : PathComputationServer {
        private readonly ConnectionController _connectionController;
        private readonly NetworkCallController _networkCallController;
        private readonly RoutingController _routingController;

        public StepByStepPathComputationServer(NetworkAddress networkAddress,
                                                 string ipAddress,
                                                 int listeningPort,
                                                 int pathComputationServerListeningPort,
                                                 int pathComputationServerDataPort) : base(networkAddress,
                                                                                           ipAddress,
                                                                                           listeningPort,
                                                                                           pathComputationServerListeningPort,
                                                                                           pathComputationServerDataPort) {
            _connectionController = new ConnectionController(networkAddress);
            _networkCallController = new NetworkCallController(networkAddress);
            _routingController = new RoutingController(networkAddress);
        }

        protected override void Receive(SignallingMessage signallingMessage) {
            throw new NotImplementedException();
        }
    }
}
