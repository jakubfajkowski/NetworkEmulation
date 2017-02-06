using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetworkUtilities.DataPlane;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class ConnectionController : ControlPlaneElement {
        private readonly Dictionary<UniqueId, NetworkAddress[]> _clientAddressDictionary =
            new Dictionary<UniqueId, NetworkAddress[]>();
        private readonly Dictionary<UniqueId, Stack<SubnetworkPointPool>> _snppStacks =
            new Dictionary<UniqueId, Stack<SubnetworkPointPool>>();

        public ConnectionController(NetworkAddress localAddress) : base(localAddress, ControlPlaneElementType.CC) {}

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.ConnectionRequest:
                    if (message.SourceControlPlaneElement.Equals(ControlPlaneElementType.NCC))
                        HandleConnectionRequestFromNCC(message);
                    else if (message.SourceControlPlaneElement.Equals(ControlPlaneElementType.CC))
                        HandleConnectionRequestFromCC(message);
                    break;

                case OperationType.PeerCoordination:
                    HandlePeerCoordination(message);
                    break;

                case OperationType.RouteTableQuery:
                    HandleRouteTableQuery(message);
                    break;

                case OperationType.LinkConnectionAllocation:
                    HandleLinkConnectionAllocation(message);
                    break;
            }
        }

        private void HandleConnectionRequestFromNCC(SignallingMessage message) {
            SendRouteTableQuery(message);
        }

        private void HandleConnectionRequestFromCC(SignallingMessage message) {
            if (message.Payload == null) {
                SendConnectionRequestResponse(message, null);
            }
            else {
                if (message.Payload is SubnetworkPoint) {
                    ProcessNextSnppPair(message);
                }
                else if (message.Payload is SubnetworkPointPool[]) {
                    var snpps = (SubnetworkPointPool[])message.Payload;
                    if (snpps[0].NodeAddress.Equals(LocalAddress)) {
                        SendLinkConnectionRequest(message);
                    }
                    else {
                        SendRouteTableQuery(message);
                    }
                }
            }
        }

        private void HandlePeerCoordination(SignallingMessage message) {
            SendRouteTableQuery(message);
        }

        private void SendPeerCoordination(SignallingMessage message) {
            var peerCoordination = message;
            peerCoordination.Operation = OperationType.PeerCoordination;
            peerCoordination.DestinationAddress = OtherDomainAddress();
            peerCoordination.DestinationControlPlaneElement = ControlPlaneElementType.CC;

            SendMessage(peerCoordination);
        }

        private void HandleRouteTableQuery(SignallingMessage message) {
            if (message.Payload is Stack<SubnetworkPointPool>) {
                var snppStack = (Stack<SubnetworkPointPool>) message.Payload;

                _snppStacks.Add(message.SessionId, snppStack);

                ProcessNextSnppPair(message);
            }
            else {
                SendConnectionRequestResponse(message, null);
            }
           
        }

        private void HandleLinkConnectionAllocation(SignallingMessage message) {
            if (message.Payload is SubnetworkPointPortPair[]) {
                var r = (SubnetworkPointPortPair[])message.Payload;
                SendConnectionRequestResponse(message, r[0].SubnetworkPoint);
            }
            else if (message.Payload is SubnetworkPointPortPair) {
                var r = (SubnetworkPointPortPair) message.Payload;
                SendConnectionRequestResponse(message, r.SubnetworkPoint);
            }
        }

        private void SendRouteTableQuery(SignallingMessage message) {
            var routeTableQuery = message;
            routeTableQuery.Operation = OperationType.RouteTableQuery;
            routeTableQuery.DestinationAddress = LocalAddress;
            routeTableQuery.DestinationControlPlaneElement = ControlPlaneElementType.RC;

            SendMessage(routeTableQuery);
        }

        private void SendConnectionRequest(SignallingMessage message, NetworkAddress destinationAddress) {
            var snpps = (SubnetworkPointPool[]) message.Payload;
            OnUpdateState($"{snpps[0]}->{snpps[1]}");

            var connectionRequest = message; 
            connectionRequest.Operation = OperationType.ConnectionRequest;
            connectionRequest.DestinationAddress = destinationAddress;
            connectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.CC;

            SendMessage(connectionRequest);
        }

        private void SendLinkConnectionRequest(SignallingMessage message) {
            var linkConnectionRequest = message;
            linkConnectionRequest.Operation = OperationType.LinkConnectionAllocation;
            linkConnectionRequest.DestinationAddress = LocalAddress;
            linkConnectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.LRM;

            SendMessage(linkConnectionRequest);
        }

        private void SendConnectionRequestResponse(SignallingMessage message, SubnetworkPoint snp) {
            var connectionRequest = message;
            connectionRequest.Payload = snp;
            connectionRequest.Operation = OperationType.ConnectionRequest;

            if (LocalAddress.IsDomain) {
                connectionRequest.DestinationAddress = LocalAddress;
                connectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.NCC;
            }
            else {
                connectionRequest.DestinationAddress = LocalAddress.GetParentsAddress();
                connectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.CC;
            }

            SendMessage(message);
        }

        public NetworkAddress OtherDomainAddress() {
            if (LocalAddress.DomainId == 1) return new NetworkAddress(2);
            if (LocalAddress.DomainId == 2) return new NetworkAddress(1);
            return null;
        }

        private void ProcessNextSnppPair(SignallingMessage message) {
            var snppStack = _snppStacks[message.SessionId];
            if (snppStack.Count > 0) {
                var snpp2 = snppStack.Pop();
                var snpp1 = snppStack.Pop();

                var snpps = new[] {
                    snpp1,
                    snpp2
                };

                message.Payload = snpps;

                var childAddress = snpps[0].NodeAddress.GetRootFromBeginning(LocalAddress.Levels + 1);
                SendConnectionRequest(message, childAddress);
            }
            else {
                SendConnectionRequestResponse(message, (SubnetworkPoint) message.Payload);
            }
        }
    }
}