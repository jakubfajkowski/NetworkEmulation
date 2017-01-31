using System;
using System.Collections.Generic;
using System.Linq;
using NetworkUtilities.ControlPlane.GraphAlgorithm;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class RoutingController : ControlPlaneElement {
        private readonly List<Link> _links = new List<Link>();

        public RoutingController(NetworkAddress networkAddress) : base(networkAddress, ControlPlaneElementType.RC) {
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.RouteTableQuery:
                    HandleRouteTableQuery(message);
                    break;
                case OperationType.LocalTopology:
                    HandleLocalTopology(message);
                    if (Address.Levels > 1)
                        SendNetworkTopology(message);
                    break;
                case OperationType.NetworkTopology:
                    HandleNetworkTopology(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SendNetworkTopology(SignallingMessage message) {
            message.Operation = OperationType.NetworkTopology;
            message.DestinationAddress = Address.GetParentsAddress();
            message.DestinationControlPlaneElement =
                ControlPlaneElementType.RC;
            SendMessage(message);
        }

        private void HandleRouteTableQuery(SignallingMessage message) {
            var snpps = (SubnetworkPointPool[]) message.Payload;
            var beginSnpp = snpps[0];
            var endSnpp = snpps[1];
            var demandedCapacity = message.DemandedCapacity;

            message.Operation = OperationType.RouteTableQueryResponse;
            message.Payload = CalculateShortestPath(beginSnpp, endSnpp, demandedCapacity);
            message.DestinationAddress = message.SourceAddress;
            message.DestinationControlPlaneElement =
                ControlPlaneElementType.CC;

            SendMessage(message);
        }

        private Queue<SubnetworkPointPool> CalculateShortestPath(SubnetworkPointPool beginSnpp,
            SubnetworkPointPool endSnpp, int demandedCapacity) {
            try {
                var beginNode = beginSnpp.NetworkNodeAddress;
                var endNode = endSnpp.NetworkNodeAddress;
                var availableLinks = _links.Where(link => link.CapacityLeft >= demandedCapacity).ToList();
                var preparedPaths = Convert(availableLinks);

                var shortestPath = Engine.CalculateShortestPathBetween(beginNode, endNode, preparedPaths);

                return Convert(shortestPath);
            }
            catch (Exception) {
                return new Queue<SubnetworkPointPool>();
            }
        }

        private List<Path<NetworkAddress>> Convert(List<Link> links) {
            var paths = new List<Path<NetworkAddress>>();

            foreach (var link in links)
                paths.Add(new Path<NetworkAddress> {
                    Source = link.BeginSubnetworkPointPool.NetworkNodeAddress,
                    Destination = link.EndSubnetworkPointPool.NetworkNodeAddress,
                    Link = link
                });

            return paths;
        }

        private Queue<SubnetworkPointPool> Convert(LinkedList<Path<NetworkAddress>> paths) {
            var subnetworkPointPools = new Queue<SubnetworkPointPool>();

            foreach (var path in paths) {
                subnetworkPointPools.Enqueue(path.Link.BeginSubnetworkPointPool);
                subnetworkPointPools.Enqueue(path.Link.EndSubnetworkPointPool);
            }

            return subnetworkPointPools;
        }

        private void HandleLocalTopology(SignallingMessage message) {
            _links.Add((Link) message.Payload);
        }

        private void HandleNetworkTopology(SignallingMessage message) {
            _links.Add((Link) message.Payload);
        }
    }
}