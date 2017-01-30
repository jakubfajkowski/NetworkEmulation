using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class StepByStepPathComputationServer : PathComputationServer {
        private readonly ConnectionController _connectionController;
        private readonly NetworkCallController _networkCallController;
        private readonly RoutingController _routingController;

        public StepByStepPathComputationServer(
            NetworkAddress networkAddress,
            string ipAddress,
            int signallingCloudListeningPort) : base(
                                                networkAddress,
                                                ipAddress,
                                                signallingCloudListeningPort) {

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