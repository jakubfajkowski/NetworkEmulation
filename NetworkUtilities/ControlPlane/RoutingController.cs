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

            message.Payload = CalculateShortestPath(beginSnpp, endSnpp, demandedCapacity);

            SendRouteTableQueryResponse(message);
        }

        private void HandleLocalTopology(SignallingMessage message) {
            UpdateLinkList(message);
            var link = (Link) message.Payload;

            if (LocalAddress.Levels == 1) {
                SendNetworkTopology(message);
            }
            else if (IsBetweenSubnetworks(link))
                SendLocalTopology(message);
        }

        private void HandleNetworkTopology(SignallingMessage message) {
            UpdateLinkList(message);
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

        private Queue<SubnetworkPointPool> CalculateShortestPath(SubnetworkPointPool beginSnpp,
            SubnetworkPointPool endSnpp, int demandedCapacity) {
            try {
                var beginNode = beginSnpp.NodeAddress.GetRootFromBeginning(LocalAddress.Levels + 1);
                var endNode = endSnpp.NodeAddress.GetRootFromBeginning(LocalAddress.Levels + 1);
                var availableLinks = _links.Where(link => link.CapacityLeft >= demandedCapacity).ToList();
                var preparedPaths = Convert(availableLinks);

                var shortestPath = Engine.CalculateShortestPathBetween(beginNode, endNode, preparedPaths);

                return Convert(beginSnpp, shortestPath, endSnpp);
            }
            catch (Exception) {
                OnUpdateState("[NO_AVAILABLE_ROUTE]");
                return new Queue<SubnetworkPointPool>();
            }
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

        private Queue<SubnetworkPointPool> Convert(SubnetworkPointPool beginSnpp, LinkedList<Path<NetworkAddress>> paths, SubnetworkPointPool endSnpp) {
            var subnetworkPointPools = new Queue<SubnetworkPointPool>();

            OnUpdateState("[AVAILABLE_ROUTE]");

            if (beginSnpp.NetworkAddress.Levels != LocalAddress.Levels + 2) {
                subnetworkPointPools.Enqueue(paths.Last.Value.Link.EndSubnetworkPointPool);
                subnetworkPointPools.Enqueue(endSnpp);
                OnUpdateState($"                   END: {endSnpp}");
            }


            foreach (var path in paths) {
                subnetworkPointPools.Enqueue(path.Link.BeginSubnetworkPointPool);
                subnetworkPointPools.Enqueue(path.Link.EndSubnetworkPointPool);

                OnUpdateState($"                   {path.Link}");
            }

            if (beginSnpp.NetworkAddress.Levels != LocalAddress.Levels + 2) {
                subnetworkPointPools.Enqueue(beginSnpp);
                subnetworkPointPools.Enqueue(paths.First.Value.Link.BeginSubnetworkPointPool);
                OnUpdateState($"                   BEGIN: {beginSnpp}");
            }

            return subnetworkPointPools;
        }

        private bool IsBetweenSubnetworks(Link link) {
            var snppA = link.BeginSubnetworkPointPool.NetworkAddress;
            var snppB = link.EndSubnetworkPointPool.NetworkAddress;

            return snppA.GetId(LocalAddress.Levels - 1) != snppB.GetId(LocalAddress.Levels - 1);
        }

        private NetworkAddress GetOtherDomainAddress(NetworkAddress localAddress) {
            if (localAddress.DomainId == 1) {
                return new NetworkAddress(2);
            }
            if (localAddress.DomainId == 2) {
                return new NetworkAddress(1);
            }
            return null;
        }

        private void UpdateLinkList(SignallingMessage message) {
            var receivedLink = (Link)message.Payload;
            OnUpdateState($"[TOPOLOGY_UPDATE] {receivedLink}");

            if (_links.Contains(receivedLink)) _links.Remove(receivedLink);
            _links.Add(receivedLink);
        }
    }
}