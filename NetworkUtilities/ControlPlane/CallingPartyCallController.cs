using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class CallingPartyCallController : ControlPlaneElement {
        private readonly NetworkAddress _nccAddress;
        private bool _callConfirmed;

        public CallingPartyCallController(NetworkAddress networkAddress)
            : base(networkAddress, ControlPlaneElementType.CPCC) {
            _nccAddress = networkAddress.GetRootFromBeginning(1);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.CallAccept:
                    //SendCallAcceptResponse(message);
                    _callConfirmed = true;
                    SendCallConfirmation(message, _callConfirmed);
                    break;

                case OperationType.CallTeardown:
                    SendCallTeardownResponse(message);
                    break;

                case OperationType.CallConfirmation:

                    break;

                case OperationType.CallRequestResponse:

                    break;

                case OperationType.CallTeardownResponse:

                    break;
            }
        }

        public void SendCallRequest(string clientA, string clientZ, int capacity) {
            string[] clientNames = {clientA, clientZ};
            var callRequest = new SignallingMessage {
                Operation = OperationType.CallRequest,
                Payload = clientNames,
                DemandedCapacity = capacity,
                DestinationAddress = _nccAddress,
                DestinationControlPlaneElement = ControlPlaneElementType.NCC
            };
            SendMessage(callRequest);
        }

        public void SendCallTeardown(string clientA, string clientZ) {
            string[] clientNames = {clientA, clientZ};
            var callTeardown = new SignallingMessage {
                Operation = OperationType.CallTeardown,
                Payload = clientNames,
                DestinationAddress = _nccAddress,
                DestinationControlPlaneElement = ControlPlaneElementType.NCC
            };
            SendMessage(callTeardown);
        }

        private void SendCallConfirmation(SignallingMessage message, bool confirmation) {
            var callConfirmation = message;
            callConfirmation.Operation = OperationType.CallConfirmation;
            callConfirmation.Payload = confirmation;
            callConfirmation.DestinationAddress = message.SourceAddress;
            callConfirmation.DestinationControlPlaneElement =
                ControlPlaneElementType.NCC;
            SendMessage(callConfirmation);
        }

        private void SendCallAcceptResponse(SignallingMessage message) {
            var callAcceptResponse = message;
            callAcceptResponse.Operation = OperationType.CallAcceptResponse;
            callAcceptResponse.Payload = true;
            callAcceptResponse.DestinationAddress = message.SourceAddress;
            SendMessage(callAcceptResponse);
        }

        private void SendCallTeardownResponse(SignallingMessage message) {
            var callTeardownResponse = message;
            callTeardownResponse.Operation = OperationType.CallTeardownResponse;
            callTeardownResponse.Payload = true;
            callTeardownResponse.DestinationAddress = _nccAddress;
            SendMessage(callTeardownResponse);
        }

        
    }
}