namespace NetworkUtilities.ControlPlane {
    public class HierarchicalPathComputationServer : PathComputationServer {
        private readonly ConnectionController _connectionController;
        private readonly RoutingController _routingController;

        public HierarchicalPathComputationServer(NetworkAddress networkAddress,
            string ipAddress,
            int listeningPort,
            int pathComputationServerListeningPort) : base(
                networkAddress,
                networkAddress.GetParentsAddress(),
                ipAddress,
                listeningPort, pathComputationServerListeningPort) {

            _connectionController = new ConnectionController(networkAddress);
            _routingController = new RoutingController(networkAddress);
        }

        protected override void Receive(SignallingMessage signallingMessage) {
            switch (signallingMessage.DestinationControlPlaneElement) {
                case SignallingMessageDestinationControlPlaneElement.ConnectionController:
                    _connectionController.ReceiveMessage(signallingMessage);
                    break;

                case SignallingMessageDestinationControlPlaneElement.RoutingController:
                    _routingController.ReceiveMessage(signallingMessage);
                    break;
            }
        }
    }
}