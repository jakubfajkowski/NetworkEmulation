using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class CallingPartyCallController : ControlPlaneElement {
        private readonly NetworkAddress _nccAddress;
        private bool _callConfirmed;

        public CallingPartyCallController(NetworkAddress localAddress)
            : base(localAddress, ControlPlaneElementType.CPCC) {
            _nccAddress = localAddress.GetRootFromBeginning(1);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.CallAccept:
                    _callConfirmed = true;
                    SendCallAccept(message, _callConfirmed);
                    break;

                case OperationType.CallConfirmation:
                    //TODO
                    break;

                case OperationType.CallTeardown:
                    SendCallTeardownResponse(message);
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

        private void SendCallAccept(SignallingMessage message, bool confirmation) {
            var callConfirmation = message;
            callConfirmation.Operation = OperationType.CallAccept;
            callConfirmation.Payload = confirmation;
            callConfirmation.DestinationAddress = message.SourceAddress;
            callConfirmation.DestinationControlPlaneElement =
                ControlPlaneElementType.NCC;

            SendMessage(callConfirmation);
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

        private void SendCallTeardownResponse(SignallingMessage message) {
        }
    }
}