using System.Collections.Generic;
using System.Windows.Forms;
using NetworkUtilities.ControlPlane.GraphAlgorithm;
using NetworkUtilities.DataPlane;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class LinkResourceManager : ControlPlaneElement {
        private readonly List<Link> _links;
        //private readonly List<SubnetworkPoint> _usedSubnetworkPoints;
        private Dictionary<int, List<SubnetworkPoint>> _usedSubnetworkPoints;
        private RecentSnp _recentSnp;
        public LinkResourceManager(NetworkAddress networkAddress) : 
            base(networkAddress, ControlPlaneElementType.LRM) {

            _links = new List<Link>();
            _usedSubnetworkPoints = new Dictionary<int, List<SubnetworkPoint>>();
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
                    var payload = message.Payload as SubnetworkPointPool[];
                    if (payload==null) 
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
            SendSnpNegotiation(message);
        }

        private void HandleSnpLinkConnectionDeallocation(SignallingMessage message) {

        }

        private void HandleConfiguration(SignallingMessage message) {
            SendLocalTopology(message);
        }

        private void HandleSnpNegotiation(SignallingMessage message) {
            var snpps = message.Payload as SubnetworkPointPool[];
            var port = snpps[0].Id;
            var snp = GenerateSnp(port, message.DemandedCapacity);
            _recentSnp = new RecentSnp(snp, snpps[1].Id);
            _usedSubnetworkPoints[snpps[1].Id].Add(snp);
            SendSnpNegotiationResponse(message, snp, port);
        }

        private void SendSnpNegotiationResponse(SignallingMessage message, SubnetworkPoint subnetworkPoint, int port) {
            message.DestinationAddress = message.SourceAddress;
            message.Payload = new object[]
                {(SubnetworkPointPool[]) message.Payload, new RecentSnp(subnetworkPoint, port)};
            SendSnpNegotiation(message);
        }

        private SubnetworkPoint GenerateSnp(int port,  int capacity ) {
            var isContinue = true;
            SubnetworkPoint subnetworkPoint= null;
            while (isContinue) {
                subnetworkPoint = SubnetworkPoint.GenerateRandom(capacity);
                foreach (var snp in _usedSubnetworkPoints[port]) {
                    if (snp.Equals(subnetworkPoint)) {
                        break;
                  }
                    isContinue = false;
                }
            }
            return subnetworkPoint;
        }
        private void HandleSnpNegotiationResponse(SignallingMessage message) {
            //var success = false;
            var payload = message.Payload as object[];
            var msg = message;
            var subnetworkPointPools = payload[0] as SubnetworkPointPool[];
            var recentSnps = payload[1] as RecentSnp[]; 


            //if (success) {
            //    SendLocalTopology(message);
            //}
            //SendSnpLinkConnectionRequest(message);
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
            message.Operation = OperationType.LocalTopology;
            message.DestinationAddress = message.DestinationAddress.GetParentsAddress();
            message.DestinationControlPlaneElement = ControlPlaneElementType.RC;
            
            SendMessage(message);
        }

        private void SendSnpLinkConnectionRequest(SignallingMessage message) {

        }

        private void SendSnpNegotiation(SignallingMessage message) {
            var snpps = message.Payload as SubnetworkPointPool[];
            message.Operation = OperationType.SNPNegotiation;
            if (snpps != null) message.DestinationAddress = snpps[1].NetworkNodeAddress;
            SendMessage(message);
        }
        private void SendSnpLinkConnectionDeallocation(SignallingMessage message) {
            
        }
    }
}