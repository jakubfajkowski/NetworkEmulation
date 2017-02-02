using System.Collections.Generic;
using System.Linq;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class LinkResourceManager : ControlPlaneElement {
        private readonly List<Link> _nodeLinks;
        private readonly Dictionary<NetworkAddress, SubnetworkPointPool> _clientInSnpps;
        private readonly Dictionary<SubnetworkPointPool, List<SubnetworkPoint>> _usedSubnetworkPoints;
        private Dictionary<UniqueId, RecentSnp> _recentSnps;

        public LinkResourceManager(NetworkAddress networkAddress) : 
            base(networkAddress, ControlPlaneElementType.LRM) {

            _nodeLinks = new List<Link>();
            _usedSubnetworkPoints = new Dictionary<SubnetworkPointPool, List<SubnetworkPoint>>();
            _clientInSnpps = new Dictionary<NetworkAddress, SubnetworkPointPool>();
            _recentSnps = new Dictionary<UniqueId, RecentSnp>();
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.SNPLinkConnectionRequest:
                    HandleSnpLinkConnectionRequest(message);
                    break;

                case OperationType.SNPNegotiation:
                    if (message.Payload is SubnetworkPointPool[])
                        HandleSnpNegotiation(message);
                    else
                        HandleSnpNegotiationResponse(message);
                    break;

                case OperationType.SNPLinkConnectionDeallocation:
                    HandleSnpLinkConnectionDeallocation(message);
                    break;

                case OperationType.SNPRelease:
                    if (message.SourceControlPlaneElement.Equals(ControlPlaneElementType.LRM))
                        HandleSnpRelease(message);
                    else
                        HandleSnpReleaseResponse(message);
                    break;

                case OperationType.Configuration:
                    HandleConfiguration(message);
                    break;
            }
        }

        private void HandleSnpLinkConnectionRequest(SignallingMessage message) {
            SendSnpNegotiation(message);
        }

        private void HandleSnpNegotiation(SignallingMessage message) {
            var snpps = message.Payload as SubnetworkPointPool[];

            var snp = GenerateSnp(snpps[1], message.DemandedCapacity);
            _recentSnps.Add(message.SessionId, new RecentSnp(snp, snpps[1].Id));
            _usedSubnetworkPoints[snpps[1]].Add(snp);

            SendSnpNegotiationResponse(message, snp);
        }

        private void HandleSnpNegotiationResponse(SignallingMessage message) {
            var payload = (object[]) message.Payload;

            var snpps = (SubnetworkPointPool[]) payload[0];
            var snp = (SubnetworkPoint) payload[1];

            var modifiedLink = _nodeLinks.First(link => link.BeginSubnetworkPointPool.Equals(snpps[0]));
            modifiedLink.ReserveCapacity(message.DemandedCapacity);
            message.Payload = modifiedLink;
            OnUpdateState($"[MODIFIED_LINK] {modifiedLink}");
            SendLocalTopology(message);
            OnUpdateState("BIFOR");
            if (_recentSnps.ContainsKey(message.SessionId)) {
                OnUpdateState("1");
                message.Payload = new[] {
                    _recentSnps[message.SessionId],
                    new RecentSnp(snp, snpps[0].Id)
                };
            }
            else {
                OnUpdateState("2");
                OnUpdateState(message.SourceClientAddress.ToString());
                foreach (var key in _clientInSnpps.Keys) {
                    OnUpdateState(key.ToString());
                }
                var clientInSnpp = _clientInSnpps[message.SourceClientAddress];
                var randomSnp = GenerateSnp(clientInSnpp, message.DemandedCapacity);
                OnUpdateState("3");
                message.Payload = new[] {
                    new RecentSnp(randomSnp, clientInSnpp.Id),
                    new RecentSnp(snp, snpps[0].Id)
                };
                OnUpdateState("4");
            }

            OnUpdateState("AFTER");
            SendSnpLinkConnectionRequest(message);
        }

        private void SendSnpNegotiationResponse(SignallingMessage message, SubnetworkPoint subnetworkPoint) {
            message.Payload = new object[]
                {(SubnetworkPointPool[]) message.Payload, subnetworkPoint};
            message.DestinationAddress = message.SourceAddress;
            message.DestinationControlPlaneElement = ControlPlaneElementType.LRM;

            SendMessage(message);
        }

        private void HandleSnpLinkConnectionDeallocation(SignallingMessage message) {

        }

        private void HandleConfiguration(SignallingMessage message) {
            var link = (Link) message.Payload;
            _nodeLinks.Add(link);
            _usedSubnetworkPoints.Add(link.BeginSubnetworkPointPool, new List<SubnetworkPoint>());

            SendLocalTopology(message);
        }

        private SubnetworkPoint GenerateSnp(SubnetworkPointPool snpp, int demandedCapacity) {
            while (true) {
                SubnetworkPoint snp = SubnetworkPoint.GenerateRandom(demandedCapacity);
                OnUpdateState($"[GENERATED_SNP] {snpp}->{snp}");
                if (!_usedSubnetworkPoints[snpp].Contains(snp))
                    return snp;
            }
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

        private void SendSnpNegotiation(SignallingMessage message) {
            var snpps = (SubnetworkPointPool[]) message.Payload;
            message.Operation = OperationType.SNPNegotiation;
            message.DestinationAddress = snpps[1].NetworkNodeAddress;
            message.DestinationControlPlaneElement = ControlPlaneElementType.LRM;

            SendMessage(message);
        }

        private void SendLocalTopology(SignallingMessage message) {
            message.Operation = OperationType.LocalTopology;
            message.DestinationAddress = LocalAddress.GetParentsAddress();
            message.DestinationControlPlaneElement = ControlPlaneElementType.RC;
            
            SendMessage(message);
        }

        private void SendSnpLinkConnectionRequest(SignallingMessage message) {
            message.DestinationAddress = LocalAddress;
            message.DestinationControlPlaneElement= ControlPlaneElementType.CC;
            message.Operation = OperationType.SNPLinkConnectionRequest;
            SendMessage(message);
        }

        private void SendSnpLinkConnectionDeallocation(SignallingMessage message) {
            
        }

        public void ConnectClient(object sender, Link link, NetworkAddress clientAddress) {
            if (link.BeginSubnetworkPointPool.NetworkNodeAddress.Equals(clientAddress)) {
                _nodeLinks.Add(link);
                _usedSubnetworkPoints.Add(link.EndSubnetworkPointPool, new List<SubnetworkPoint>());
                _clientInSnpps.Add(clientAddress, link.EndSubnetworkPointPool);
            }
            else {
                _nodeLinks.Add(link);
                _usedSubnetworkPoints.Add(link.BeginSubnetworkPointPool, new List<SubnetworkPoint>());
                _clientInSnpps.Add(clientAddress, link.BeginSubnetworkPointPool);
            }

            var message = new SignallingMessage {
                Payload = link
            };
            SendLocalTopology(message);
        }
    }
}