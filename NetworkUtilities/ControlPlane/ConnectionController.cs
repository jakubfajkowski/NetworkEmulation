using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class ConnectionController : ControlPlaneElement {
        public delegate void CommutationHandler(object sender, CommutationHandlerArgs args);

        private readonly Dictionary<UniqueId, NetworkAddress[]> _snppDictionary =
            new Dictionary<UniqueId, NetworkAddress[]>();

        private Queue<SubnetworkPointPool> _snpPools;

        public ConnectionController(NetworkAddress networkAddress) : base(networkAddress, ControlPlaneElementType.CC) {
            snPoint = SubnetworkPoint.GenerateRandom(100);
            snPointCpcc = snPoint;
            Port = 1;
        }
        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.ConnectionRequest:
                    HandleConnectionReques(message);
                    break;

                case OperationType.PeerCoordination:
                    SendRouteTableQuery(message);
                    break;

                case OperationType.RouteTableQueryResponse:
                    HandleRouteTableQueryResponse(message);
                    break;

                case OperationType.ConnectionRequestResponse:
                    HandleConnectionRequestResponse(message);
                    break;

                case OperationType.SetLabels:
                    var labels = (int[])message.Payload;
                    Debug.WriteLine("Received VPI: " + labels[0] + ", VCI: " + labels[1]);
                    break;

                case OperationType.GetLabelsFromLRM:
                    break;

                case OperationType.LinkConnectionResponse:
                    HandleLinkConnectionResponse(message);
                    break;
            }
        }

        public SubnetworkPoint snPoint { get; set; }
        public SubnetworkPoint snPointCpcc { get; set; }
        public int Port { get; set; }

        

        private void HandleConnectionReques(SignallingMessage message) {
            var snpp = (SubnetworkPointPool[])message.Payload;
            var snppAddressA = snpp[0].NetworkAddress;
            var snppAddressB = snpp[1].NetworkAddress;
            NetworkAddress[] snppAddress = { snppAddressA, snppAddressB };
            _snppDictionary.Add(message.SessionId, snppAddress);

            if (message.DestinationAddress.Levels == _snppDictionary[message.SessionId][0].Levels - 1)
                SendLinkConnectionRequest(message);

            if (message.SourceAddress.Equals(new NetworkAddress("1")) ||
                message.SourceAddress.Equals(new NetworkAddress("2")))
                if (message.SourceAddress.GetRootFromBeginning(1).Equals(_snppDictionary[message.SessionId][1]))
                    SendPeerCoordination(message);
                else SendRouteTableQuery(message);
            else SendRouteTableQuery(message);
        }

        private void SendLinkConnectionRequest(SignallingMessage message) {
            message.Operation = OperationType.SNPLinkConnectionRequest;
            message.DestinationAddress = _snppDictionary[message.SessionId][1];
            message.DestinationControlPlaneElement = ControlPlaneElementType.LRM;
            SendMessage(message);
        }

        private void SendRouteTableQuery(SignallingMessage message) {
            message.Operation = OperationType.RouteTableQuery;
            message.DestinationAddress = Address;
            message.DestinationControlPlaneElement = ControlPlaneElementType.RC;
            SendMessage(message);
        }

        private void SendPeerCoordination(SignallingMessage message) {
            var peerCoordination = message;
            peerCoordination.Operation = OperationType.ConnectionConfirmation;
            if (message.SourceAddress.Equals(new NetworkAddress("1"))) {
                peerCoordination.DestinationAddress = new NetworkAddress("2");
                peerCoordination.DestinationControlPlaneElement =
                    ControlPlaneElementType.CC;
            }
            else {
                peerCoordination.DestinationAddress = new NetworkAddress("1");
            }
            SendMessage(peerCoordination);
        }

        private void HandleConnectionRequestResponse(SignallingMessage msg) {
            if (_snpPools == null) {
                if (msg.DestinationAddress.Levels == 1) {
                    msg.Operation = OperationType.ConnectionConfirmationToNCC;
                    msg.Payload = snPointCpcc;
                    msg.DestinationAddress = _snppDictionary[msg.SessionId][0].GetRootFromBeginning(1);
                    msg.DestinationControlPlaneElement = ControlPlaneElementType.NCC;
                    //msg.Payload = true;
                    SendMessage(msg);
                }
                else {
                    msg.DestinationAddress = msg.DestinationAddress.GetParentsAddress();
                    msg.DestinationControlPlaneElement = ControlPlaneElementType.CC;
                    SendMessage(msg);
                }
            }
            else if (Convert.ToBoolean(msg.Payload)) {
                HandleRouteTableQueryResponse(msg);
            }
            else {
                msg.DestinationAddress.GetParentsAddress();
                msg.Operation = OperationType.ConnectionRequestResponse;
                msg.DestinationControlPlaneElement = ControlPlaneElementType.CC;
                SendMessage(msg);
            }
        }

        private void HandleLinkConnectionResponse(SignallingMessage message) {
            var snp = message.Payload as SubnetworkPoint;
            OnCommutationCommand(
                new CommutationHandlerArgs(new CommutationTableRow(snPoint.Vpi, snPoint.Vpi, Port,
                    snp.Vpi, snp.Vci, message.SourceAddress.GetLastId())));
            message.Payload = true;
            message.DestinationAddress = message.DestinationAddress.GetParentsAddress();
            message.DestinationControlPlaneElement = ControlPlaneElementType.CC;
            message.Operation = OperationType.ConnectionRequestResponse;
            SendMessage(message);
        }

        private void HandleSetSnp(SignallingMessage message) {
            snPoint = message.Payload as SubnetworkPoint;
            Port = message.SourceAddress.GetLastId();
        }

        public event CommutationHandler CommutationCommand;


        public void SendGetLabelsMessage() {
            //SendMessage(new SignallingMessage(SignallingMessageOperation.SNPLinkConnectionDeallocation, 1));
        }

        private void HandleRouteTableQueryResponse(SignallingMessage msg) {
            if (msg.Operation.Equals(OperationType.RouteTableQueryResponse))
                _snpPools = msg.Payload as Queue<SubnetworkPointPool>;
            if (_snpPools.Count > 0) {
                var snpps = new[] {
                    _snpPools.Dequeue(),
                    _snpPools.Dequeue()
                };
                var levelsOfAddress = msg.DestinationAddress.Levels + 1;
                msg.Operation = OperationType.ConnectionRequest;
                msg.DestinationAddress = snpps[0].NetworkAddress.GetRootFromBeginning(levelsOfAddress);
                msg.DestinationControlPlaneElement = ControlPlaneElementType.CC;
                msg.Payload = snpps;
                SendMessage(msg);
            }
            else {
                msg.Payload = false;
                if (msg.DestinationAddress.Levels == 1) {
                    msg.Operation = OperationType.ConnectionConfirmationToNCC;
                    msg.DestinationAddress = _snppDictionary[msg.SessionId][0].GetRootFromBeginning(1);
                    msg.DestinationControlPlaneElement = ControlPlaneElementType.NCC;
                }
                else {
                    msg.DestinationAddress = msg.DestinationAddress.GetParentsAddress();
                    msg.Operation = OperationType.ConnectionRequestResponse;
                    msg.DestinationControlPlaneElement = ControlPlaneElementType.CC;
                }
                SendMessage(msg);
            }
        }

        protected virtual void OnCommutationCommand(CommutationHandlerArgs args) {
            CommutationCommand?.Invoke(this, args);
        }
    }

    public class CommutationHandlerArgs {
        public CommutationHandlerArgs(CommutationTableRow commutationTableRow) {
            CommutationTableRow = commutationTableRow;
        }

        public CommutationTableRow CommutationTableRow { get; private set; }
    }
}