namespace NetworkUtilities.ControlPlane {
    public class CallingPartyCallController : ControlPlaneElement {
        public CallingPartyCallController(NetworkAddress networkAddress) : base(networkAddress) {
        }

        public bool CallConfirmed { get; set; }
        public NetworkAddress NccAddress { get; private set; }

        public void SendCallRequest(string clientA, string clientZ, NetworkAddress nccAddress, int capacity) {
            NccAddress = nccAddress;
            string[] clientNames = {clientA, clientZ};
            object[] callRequestMessage = {clientNames, capacity};
            var callRequest = new SignallingMessage {
                Operation = SignallingMessageOperation.CallRequest,
                Payload = callRequestMessage,
                DestinationAddress = nccAddress,
                DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.NetworkCallController
            };
            SendMessage(callRequest);
        }

        public void SendCallTeardown(string clientA, string clientZ) {
            string[] clientNames = {clientA, clientZ};
            var callTeardown = new SignallingMessage {
                Operation = SignallingMessageOperation.CallTeardown,
                Payload = clientNames,
                DestinationAddress = NccAddress,
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
            callTeardownResponse.DestinationAddress = NccAddress;
            SendMessage(callTeardownResponse);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case SignallingMessageOperation.CallAccept:
                    //SendCallAcceptResponse(message);
                    CallConfirmed = true;
                    SendCallConfirmation(message, CallConfirmed);
                    break;
                case SignallingMessageOperation.CallTeardown:
                    SendCallTeardownResponse(message);
                    break;
                case SignallingMessageOperation.CallConfirmation:
                    // u klienta okienko że połączenie potwierdzone
                    break;
                case SignallingMessageOperation.CallRequestResponse:

                    break;
                case SignallingMessageOperation.CallTeardownResponse:

                    break;
            }
        }
    }
}