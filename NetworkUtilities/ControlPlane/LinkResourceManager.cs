using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class LinkResourceManager : ControlPlaneElement {
        private readonly List<Link> _nodeLinks;
        private readonly Dictionary<NetworkAddress, SubnetworkPointPool> _clientInSnpps;
        private readonly Dictionary<NetworkAddress, SubnetworkPointPool> _clientOutSnpps;
        private readonly Dictionary<SubnetworkPointPool, List<SubnetworkPoint>> _usedSubnetworkPoints;
        private readonly Dictionary<UniqueId, SubnetworkPointPortPair> _recentSnps;

        public LinkResourceManager(NetworkAddress networkAddress) : 
            base(networkAddress, ControlPlaneElementType.LRM) {

            _nodeLinks = new List<Link>();
            _usedSubnetworkPoints = new Dictionary<SubnetworkPointPool, List<SubnetworkPoint>>();
            _clientInSnpps = new Dictionary<NetworkAddress, SubnetworkPointPool>();
            _clientOutSnpps = new Dictionary<NetworkAddress, SubnetworkPointPool>();
            _recentSnps = new Dictionary<UniqueId, SubnetworkPointPortPair>();
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.SNPLinkConnectionAllocation:
                    HandleSnpLinkConnectionAllocation(message);
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

        private void HandleSnpLinkConnectionAllocation(SignallingMessage message) {
            var snpps = (SubnetworkPointPool[]) message.Payload;
            OnUpdateState($"[RECEIVED_SNPP] {snpps[0]}---{snpps[1]}");

            SendSnpNegotiation(message);
        }

        private void HandleSnpNegotiation(SignallingMessage message) {
            var snpps = (SubnetworkPointPool[]) message.Payload;

            SubnetworkPointPortPair pair;
            if (_recentSnps.ContainsKey(message.SessionId)) {
                pair = _recentSnps[message.SessionId];
                //message.DestinationAddress;
            }
            else {
                var snp = GenerateSnp(snpps[1], message.DemandedCapacity);
                pair = new SubnetworkPointPortPair(snp, snpps[1].Id);
                _recentSnps.Add(message.SessionId, pair);
                //TODO CONFIGURATION BUGFIX
                _usedSubnetworkPoints[snpps[1]].Add(snp);

                if (_clientOutSnpps.ContainsKey(message.DestinationClientAddress)) {
                    if (_clientOutSnpps[message.SourceClientAddress].NodeAddress.Equals(LocalAddress)) {
                        OnUpdateState("[DESTINATION_CLIENT_LOCATED]");
                        var snppOut = _clientOutSnpps[message.DestinationClientAddress];
                        var snpOut = GenerateSnp(snppOut, message.DemandedCapacity);
                        var pairOut = new SubnetworkPointPortPair(snpOut, snppOut.Id);
                        SendMessage(new SignallingMessage {
                            DestinationAddress = LocalAddress,
                            DestinationControlPlaneElement = ControlPlaneElementType.CC,
                            Operation = OperationType.SNPLinkConnectionAllocation,
                            Payload = new[] { pair, pairOut }
                        });
                    }
                }
            }

            
            SendSnpNegotiationResponse(message, pair);
        }

        private void SendSnpNegotiationResponse(SignallingMessage message, SubnetworkPointPortPair subnetworkPointPortPair) {
            message.Payload = new object[]
                {(SubnetworkPointPool[]) message.Payload, subnetworkPointPortPair};
            message.DestinationAddress = message.SourceAddress;
            message.DestinationControlPlaneElement = ControlPlaneElementType.LRM;

            SendMessage(message);
        }

        private void HandleSnpNegotiationResponse(SignallingMessage message) {
            var payload = (object[])message.Payload;

            var snpps = (SubnetworkPointPool[])payload[0];
            var pair2 = (SubnetworkPointPortPair)payload[1];

            var modifiedLink = _nodeLinks.First(link => link.BeginSubnetworkPointPool.Equals(snpps[0]));
            modifiedLink.ReserveCapacity(message.DemandedCapacity);
            message.Payload = modifiedLink;
            OnUpdateState($"[MODIFIED_LINK] {modifiedLink}");
            SendLocalTopology(modifiedLink);
            

            SubnetworkPointPortPair pair1 = null;
            if (_clientInSnpps.ContainsKey(message.SourceClientAddress)) {
                if (_clientInSnpps[message.SourceClientAddress].NodeAddress.Equals(LocalAddress)) {
                    OnUpdateState("[SOURCE_CLIENT_LOCATED]");
                    var routerSnpp = _clientInSnpps[message.SourceClientAddress];
                    var newSnp = GenerateSnp(routerSnpp, message.DemandedCapacity);
                    _usedSubnetworkPoints[routerSnpp].Add(newSnp);
                    pair1 = new SubnetworkPointPortPair(newSnp, routerSnpp.Id);
                }
            }
            else {
                var newSnp = GenerateSnp(snpps[0], message.DemandedCapacity);
                _usedSubnetworkPoints[snpps[0]].Add(newSnp);
                pair1 = new SubnetworkPointPortPair(newSnp, snpps[0].Id);
            }

            message.Payload = new[] {
                pair1,
                pair2
            };
            
            SendSnpLinkConnectionRequest(message);
        }

        private void HandleSnpLinkConnectionDeallocation(SignallingMessage message) {

        }

        private void HandleConfiguration(SignallingMessage message) {
            var link = (Link) message.Payload;
            OnUpdateState($"[LINK] {link}");

            _nodeLinks.Add(link);
            _usedSubnetworkPoints.Add(link.BeginSubnetworkPointPool, new List<SubnetworkPoint>());

            if (link.IsClientLink) {
                NetworkAddress clientAddress;
                SubnetworkPointPool clientConnectedLocalSnpp;

                if (link.EndSubnetworkPointPool.NodeAddress.Equals(LocalAddress)) {
                    clientAddress = link.BeginSubnetworkPointPool.NodeAddress;
                    clientConnectedLocalSnpp = link.EndSubnetworkPointPool;
                    _clientInSnpps.Add(clientAddress, clientConnectedLocalSnpp);
                }
                else {
                    clientAddress = link.EndSubnetworkPointPool.NodeAddress;
                    clientConnectedLocalSnpp = link.BeginSubnetworkPointPool;
                    _clientOutSnpps.Add(clientAddress, clientConnectedLocalSnpp);
                }
            }
            else {
                SendLocalTopology(link);
            }

            EndSession(message.SessionId);
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
                
            }
            SendSnpLinkConnectionDeallocation(message);
        }

        private void SendSnpNegotiation(SignallingMessage message) {
            var snpps = (SubnetworkPointPool[]) message.Payload;
            message.Operation = OperationType.SNPNegotiation;
            message.DestinationAddress = snpps[1].NodeAddress;
            message.DestinationControlPlaneElement = ControlPlaneElementType.LRM;

            SendMessage(message);
        }

        private void SendLocalTopology(Link link) {
            var message = new SignallingMessage {
                Operation = OperationType.LocalTopology,
                DestinationAddress = LocalAddress.GetParentsAddress(),
                DestinationControlPlaneElement = ControlPlaneElementType.RC,
                Payload = link
            };
            
            SendMessage(message);

        }

        private void SendSnpLinkConnectionRequest(SignallingMessage message) {
            message.DestinationAddress = LocalAddress;
            message.DestinationControlPlaneElement= ControlPlaneElementType.CC;
            message.Operation = OperationType.SNPLinkConnectionAllocation;

            SendMessage(message);
        }

        private void SendSnpLinkConnectionDeallocation(SignallingMessage message) {
            
        }
    }
}