using System;
using System.Collections.Generic;
using System.Linq;
using NetworkUtilities.ControlPlane.GraphAlgorithm;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class RoutingController : ControlPlaneElement {
        private readonly List<Link> _links = new List<Link>();

        public RoutingController(NetworkAddress localAddress) : base(localAddress, ControlPlaneElementType.RC) {
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.RouteTableQuery:
                    HandleRouteTableQuery(message);
                    break;
                case OperationType.LocalTopology:
                    HandleLocalTopology(message);
                    break;
                case OperationType.NetworkTopology:
                    HandleNetworkTopology(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleRouteTableQuery(SignallingMessage message) {
            var snpps = (SubnetworkPointPool[]) message.Payload;
            var beginSnpp = snpps[0];
            var endSnpp = snpps[1];
            var demandedCapacity = message.DemandedCapacity;

            try {
                message.Payload = CalculateShortestPath(beginSnpp, endSnpp, demandedCapacity);
            }
            catch (Exception) {
                OnUpdateState("[NO_AVAILABLE_ROUTE]");
                message.Payload = null;
            }

            SendRouteTableQueryResponse(message);
        }

        private void HandleLocalTopology(SignallingMessage message) {
            var link = (Link) message.Payload;
            var updated = UpdateLinkList(link);

            if (!updated) return;

            OnUpdateState($"[TOPOLOGY_UPDATE] {link}");

            if (LocalAddress.Levels == 1)
                SendNetworkTopology(message);
            else if (IsBetweenSubnetworks(link))
                SendLocalTopology(message);
        }

        private void HandleNetworkTopology(SignallingMessage message) {
            var link = (Link) message.Payload;
            if (UpdateLinkList(link))
                OnUpdateState($"[TOPOLOGY_UPDATE] {link}");
        }

        private void SendLocalTopology(SignallingMessage message) {
            message.Operation = OperationType.LocalTopology;
            message.DestinationAddress = LocalAddress.GetParentsAddress();
            message.DestinationControlPlaneElement =
                ControlPlaneElementType.RC;

            SendMessage(message);
            EndSession(message.SessionId);
        }

        private void SendNetworkTopology(SignallingMessage message) {
            message.Operation = OperationType.NetworkTopology;
            message.DestinationAddress = GetOtherDomainAddress(LocalAddress);
            message.DestinationControlPlaneElement =
                ControlPlaneElementType.RC;

            SendMessage(message);
            EndSession(message.SessionId);
        }

        private void SendRouteTableQueryResponse(SignallingMessage message) {
            message.Operation = OperationType.RouteTableQuery;
            message.DestinationAddress = message.SourceAddress;
            message.DestinationControlPlaneElement =
                ControlPlaneElementType.CC;

            SendMessage(message);
            EndSession(message.SessionId);
        }

        private Stack<SubnetworkPointPool> CalculateShortestPath(SubnetworkPointPool beginSnpp,
            SubnetworkPointPool endSnpp, int demandedCapacity) {
            var beginNode = beginSnpp.NodeAddress.GetRootFromBeginning(LocalAddress.Levels + 1);
            var endNode = endSnpp.NodeAddress.GetRootFromBeginning(LocalAddress.Levels + 1);
            var availableLinks = _links.Where(link => link.CapacityLeft >= demandedCapacity).ToList();
            var preparedPaths = Convert(availableLinks);

            var shortestPath = Engine.CalculateShortestPathBetween(beginNode, endNode, preparedPaths);

            return Convert(beginSnpp, shortestPath, endSnpp);
        }

        private List<Path<NetworkAddress>> Convert(List<Link> links) {
            var paths = new List<Path<NetworkAddress>>();

            foreach (var link in links)
                paths.Add(new Path<NetworkAddress> {
                    Source = link.BeginSubnetworkPointPool.NodeAddress.GetRootFromBeginning(LocalAddress.Levels + 1),
                    Destination = link.EndSubnetworkPointPool.NodeAddress.GetRootFromBeginning(LocalAddress.Levels + 1),
                    Link = link
                });

            return paths;
        }

        private Stack<SubnetworkPointPool> Convert(SubnetworkPointPool beginSnpp, LinkedList<Path<NetworkAddress>> paths,
            SubnetworkPointPool endSnpp) {
            var subnetworkPointPools = new Stack<SubnetworkPointPool>();

            OnUpdateState("[AVAILABLE_ROUTE]");

            OnUpdateState($"                   BEGIN: {beginSnpp}");
            if (paths.Count > 0) {
                if (LocalAddress.IsDomain) {
                    var firstPathSnpp = paths.First.Value.Link.BeginSubnetworkPointPool;
                    if (!firstPathSnpp.Equals(beginSnpp)) {
                        subnetworkPointPools.Push(beginSnpp);
                        //subnetworkPointPools.Push(firstPathSnpp);
                        OnUpdateState($"                   {beginSnpp}->{firstPathSnpp}");
                    }
                    foreach (var path in paths) {
                        if (
                            !beginSnpp.NodeAddress.GetRootFromBeginning(LocalAddress.Levels + 1)
                                .Equals(
                                    path.Link.BeginSubnetworkPointPool.NodeAddress.GetRootFromBeginning(
                                        LocalAddress.Levels + 1)))
                            subnetworkPointPools.Push(path.Link.BeginSubnetworkPointPool);
                        subnetworkPointPools.Push(path.Link.EndSubnetworkPointPool);

                        OnUpdateState($"                   {path.Link}");
                    }
                }
                else {
                    foreach (var path in paths) {
                        subnetworkPointPools.Push(path.Link.BeginSubnetworkPointPool);
                        subnetworkPointPools.Push(path.Link.EndSubnetworkPointPool);

                        OnUpdateState($"                   {path.Link}");
                    }
                }

                var lastPathSnpp = paths.Last.Value.Link.EndSubnetworkPointPool;

                if (!lastPathSnpp.Equals(endSnpp)) {
                    subnetworkPointPools.Push(lastPathSnpp);
                    subnetworkPointPools.Push(endSnpp);
                    OnUpdateState($"                   {lastPathSnpp}->{endSnpp}");
                }
            }
            else if (beginSnpp.NodeAddress.Equals(endSnpp.NodeAddress)) {
                subnetworkPointPools.Push(beginSnpp);
                subnetworkPointPools.Push(endSnpp);
            }
            OnUpdateState($"                   END: {endSnpp}");

            return subnetworkPointPools;
        }

        private bool IsBetweenSubnetworks(Link link) {
            var snppA = link.BeginSubnetworkPointPool.NetworkAddress;
            var snppB = link.EndSubnetworkPointPool.NetworkAddress;

            return snppA.GetId(LocalAddress.Levels - 1) != snppB.GetId(LocalAddress.Levels - 1);
        }

        private NetworkAddress GetOtherDomainAddress(NetworkAddress localAddress) {
            if (localAddress.DomainId == 1) return new NetworkAddress(2);
            if (localAddress.DomainId == 2) return new NetworkAddress(1);
            return null;
        }

        private bool UpdateLinkList(Link receivedLink) {
            var updatedLinkIndex = _links.FindIndex(link => link.Equals(receivedLink));
            if (updatedLinkIndex >= 0) {
                if (_links[updatedLinkIndex].CapacityLeft != receivedLink.CapacityLeft) {
                    _links[updatedLinkIndex] = receivedLink;
                    return true;
                }

                return false;
            }

            _links.Add(receivedLink);
            return true;
        }
    }
}