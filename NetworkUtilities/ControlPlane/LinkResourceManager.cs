using System.Collections.Generic;
using NetworkUtilities.ControlPlane.GraphAlgorithm;
using NetworkUtilities.DataPlane;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class LinkResourceManager : ControlPlaneElement {
        private readonly List<Link> _links;
        private readonly List<SubnetworkPoint> _usedSubnetworkPoints;


        public LinkResourceManager(NetworkAddress networkAddress) : 
            base(networkAddress, ControlPlaneElementType.LRM) {

            _links = new List<Link>();
            _usedSubnetworkPoints = new List<SubnetworkPoint>();
        }

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case OperationType.SNPLinkConnectionRequest:
                    HandleSnpLinkConnectionRequest(message);
                    break;

                case OperationType.SNPLinkConnectionDeallocation:
                    HandleSnpLinkConnectionDeallocation(message);
                    break;

                case OperationType.Configuration:
                    HandleConfiguration(message);
                    break;

                case OperationType.SNPNegotiation:
                    if (message.SourceControlPlaneElement.Equals(ControlPlaneElementType.LRM)) 
                        HandleSnpNegotiationResponse(message);
                    else
                        HandleSnpNegotiation(message);
                    break;

                case OperationType.SNPRelease:
                    if (message.SourceControlPlaneElement.Equals(ControlPlaneElementType.LRM))
                        HandleSnpRelease(message);
                    else
                        HandleSnpReleaseResponse(message);
                    break;
            }
        }

        private void HandleSnpLinkConnectionRequest(SignallingMessage message) {

        }

        private void HandleSnpLinkConnectionDeallocation(SignallingMessage message) {

        }

        private void HandleConfiguration(SignallingMessage message) {


        }

        private void HandleSnpNegotiation(SignallingMessage message) {

        }

        private void HandleSnpNegotiationResponse(SignallingMessage message) {
            var success = false;



            if (success) {
                SendLocalTopology(message);
            }
            SendSnpLinkConnectionRequest(message);
        }

        private void HandleSnpRelease(SignallingMessage message) {

        }

        private void HandleSnpReleaseResponse(SignallingMessage message) {
            var success = false;



            if (success) {
                SendLocalTopology(message);
            }
            SendSnpLinkConnectionDeallocation(message);
        }

        private void SendLocalTopology(SignallingMessage message) {

        }

        private void SendSnpLinkConnectionRequest(SignallingMessage message) {

        }

        private void SendSnpLinkConnectionDeallocation(SignallingMessage message) {
            
        }
    }
}