using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class HierarchicalPathComputationServer : PathComputationServer {
        private ConnectionController _connectionController;
        private RoutingController _routingController;

        public HierarchicalPathComputationServer(NetworkAddress networkAddress,
            string ipAddress,
            int signallingCloudListeningPort) : base(networkAddress, ipAddress, signallingCloudListeningPort) {

            _connectionController = new ConnectionController(networkAddress);
            Initialize(_connectionController);

            _routingController = new RoutingController(networkAddress);
            Initialize(_routingController);
        }

        protected override void Receive(SignallingMessage signallingMessage) {
            switch (signallingMessage.DestinationControlPlaneElement) {
                case ControlPlaneElementType.CC:
                    _connectionController.ReceiveMessage(signallingMessage);
                    break;

                case ControlPlaneElementType.RC:
                    _routingController.ReceiveMessage(signallingMessage);
                    break;
            }
        }

        public override void Dispose() {
            base.Dispose();

            _connectionController = new ConnectionController(NetworkAddress);
            Initialize(_connectionController);

            _routingController = new RoutingController(NetworkAddress);
            Initialize(_routingController);
        }
    }
}