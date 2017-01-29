using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public class StepByStepPathComputationServer : PathComputationServer {
        private readonly ConnectionController _connectionController;

        private readonly ConnectionComponent _nameServerConnectionComponent;
        private readonly NetworkCallController _networkCallController;
        private readonly RoutingController _routingController;

        public StepByStepPathComputationServer(
            NetworkAddress networkAddress, 
            NetworkAddress peerPathComputationServerNetworkAddress,
            string ipAddress,
            int listeningPort,
            int pathComputationServerListeningPort,
            int nameServerListeningPort) : base(
                                                networkAddress,
                                                peerPathComputationServerNetworkAddress,
                                                ipAddress,
                                                listeningPort,
                                                pathComputationServerListeningPort) {

            _connectionController = new ConnectionController(networkAddress);
            _connectionController.UpdateState += (sender, state) => OnUpdateState(state);
            _connectionController.MessageToSend +=
                (sender, message) => SendSignallingMessage(message, message.DestinationAddress);

            _networkCallController = new NetworkCallController(networkAddress);
            _networkCallController.UpdateState += (sender, state) => OnUpdateState(state);
            _networkCallController.MessageToSend +=
                (sender, message) => SendSignallingMessage(message, message.DestinationAddress);

            _routingController = new RoutingController(networkAddress);
            _routingController.UpdateState += (sender, state) => OnUpdateState(state);
            _routingController.MessageToSend +=
                (sender, message) => SendSignallingMessage(message, message.DestinationAddress);

            _nameServerConnectionComponent = new ConnectionComponent(networkAddress, NameServer.Address, ipAddress, nameServerListeningPort);
            _nameServerConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
        }

        public override void Initialize() {
            base.Initialize();

            _nameServerConnectionComponent.Initialize();
        }

        protected override void Receive(SignallingMessage signallingMessage) {
            switch (signallingMessage.DestinationControlPlaneElement) {
                case SignallingMessageDestinationControlPlaneElement.NetworkCallController:
                    _networkCallController.ReceiveMessage(signallingMessage);
                    break;

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