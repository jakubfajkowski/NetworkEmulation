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
            _connectionController.UpdateState += (sender, state) => OnUpdateState(state);
            _connectionController.MessageToSend += (sender, message) => Send(message, message.DestinationAddress);

            _routingController = new RoutingController(networkAddress);
            _routingController.UpdateState += (sender, state) => OnUpdateState(state);
            _routingController.MessageToSend += (sender, message) => Send(message, message.DestinationAddress);
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