using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class Policy : ControlPlaneElement {
        public Policy(NetworkAddress networkAddress) : base(networkAddress, ControlPlaneElementType.Policy) {
        }

        private void SendPolicyResponse(SignallingMessage message) {
            var policyResponse = message;
            policyResponse.Operation = OperationType.PolicyResponse;
            policyResponse.Payload = true;
            policyResponse.DestinationAddress = message.SourceAddress;
            policyResponse.DestinationControlPlaneElement = ControlPlaneElementType.NCC;
            SendMessage(policyResponse);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);
            SendPolicyResponse(message);
        }
    }
}