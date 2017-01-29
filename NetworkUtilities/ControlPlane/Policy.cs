using System.Drawing.Text;

namespace NetworkUtilities.ControlPlane {
    internal class Policy : ControlPlaneElement {
        public Policy(NetworkAddress networkAddress) : base(networkAddress) {}

        private void SendPolicyResponse(SignallingMessage message) {
            var policyResponse = message;
            policyResponse.Operation = SignallingMessageOperation.PolicyResponse;
            policyResponse.Payload = (bool) true;
            policyResponse.DestinationAddress = message.SourceAddress;
            SendMessage(policyResponse);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            SendPolicyResponse(message);
        }
    }
}
