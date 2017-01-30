using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class CallingPartyCallController : ControlPlaneElement {
        public CallingPartyCallController(NetworkAddress networkAddress)
            : base(networkAddress) {

            _nccAddress = networkAddress.GetRootFromBeginning(1);
        }

        private bool _callConfirmed;
        private readonly NetworkAddress _nccAddress;

        public void SendCallRequest(string clientA, string clientZ, int capacity) {
            string[] clientNames = {clientA, clientZ};
            var callRequest = new SignallingMessage {
                Operation = SignallingMessageOperation.CallRequest,
                Payload = clientNames,
                DemandedCapacity = capacity,
                DestinationAddress = _nccAddress,
                DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.NetworkCallController
            };
            SendMessage(callRequest);
        }

        public void SendCallTeardown(string clientA, string clientZ) {
            string[] clientNames = {clientA, clientZ};
            var callTeardown = new SignallingMessage {
                Operation = SignallingMessageOperation.CallTeardown,
                Payload = clientNames,
                DestinationAddress = _nccAddress,
                DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.NetworkCallController
            };
            SendMessage(callTeardown);
        }

        private void SendCallConfirmation(SignallingMessage message, bool confirmation) {
            var callConfirmation = message;
            callConfirmation.Operation = SignallingMessageOperation.CallConfirmation;
            callConfirmation.Payload = confirmation;
            callConfirmation.DestinationAddress = message.SourceAddress;
            callConfirmation.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.NetworkCallController;
            SendMessage(callConfirmation);
        }

        private void SendCallAcceptResponse(SignallingMessage message) {
            var callAcceptResponse = message;
            callAcceptResponse.Operation = SignallingMessageOperation.CallAcceptResponse;
            callAcceptResponse.Payload = true;
            callAcceptResponse.DestinationAddress = message.SourceAddress;
            SendMessage(callAcceptResponse);
        }

        private void SendCallTeardownResponse(SignallingMessage message) {
            var callTeardownResponse = message;
            callTeardownResponse.Operation = SignallingMessageOperation.CallTeardownResponse;
            callTeardownResponse.Payload = true;
            callTeardownResponse.DestinationAddress = _nccAddress;
            SendMessage(callTeardownResponse);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case SignallingMessageOperation.CallAccept:
                    //SendCallAcceptResponse(message);
                    _callConfirmed = true;
                    SendCallConfirmation(message, _callConfirmed);
                    break;
                case SignallingMessageOperation.CallTeardown:
                    SendCallTeardownResponse(message);
                    break;
                case SignallingMessageOperation.CallConfirmation:
                    
                    break;
                case SignallingMessageOperation.CallRequestResponse:

                    break;
                case SignallingMessageOperation.CallTeardownResponse:

                    break;
            }
        }
    }
}