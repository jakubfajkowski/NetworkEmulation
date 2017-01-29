using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public class StepByStepPathComputationServer : PathComputationServer {
        private readonly NetworkCallController _networkCallController;
        private readonly ConnectionController _connectionController;
        private readonly RoutingController _routingController;

        private readonly ConnectionComponent _nameServerConnectionComponent;

        public StepByStepPathComputationServer(NetworkAddress networkAddress,
                                                 string ipAddress,
                                                 int listeningPort,
                                                 int pathComputationServerListeningPort,
                                                 int nameServerListeningPort) : base(networkAddress,
                                                                                           ipAddress,
                                                                                           listeningPort,
                                                                                           pathComputationServerListeningPort) {
            _connectionController = new ConnectionController(networkAddress);
            _networkCallController = new NetworkCallController(networkAddress);
            _routingController = new RoutingController(networkAddress);

            _nameServerConnectionComponent = new ConnectionComponent(networkAddress, ipAddress, nameServerListeningPort);
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
