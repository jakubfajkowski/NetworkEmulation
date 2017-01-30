using System.Drawing.Text;

namespace NetworkUtilities.ControlPlane {
    public class Policy : ControlPlaneElement {
        public Policy(NetworkAddress networkAddress) : base(networkAddress) {}

        private void SendPolicyResponse(SignallingMessage message) {
            var policyResponse = message;
            policyResponse.Operation = SignallingMessageOperation.PolicyResponse;
            policyResponse.Payload = true;
            policyResponse.DestinationAddress = message.SourceAddress;
            policyResponse.DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.NetworkCallController;
            SendMessage(policyResponse);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);
            SendPolicyResponse(message);
        }
    }
}
