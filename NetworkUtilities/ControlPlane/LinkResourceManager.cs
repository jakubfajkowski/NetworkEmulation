using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetworkUtilities.DataPlane;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class LinkResourceManager : ControlPlaneElement {
        public static int MaxLabelNumber = 1000;
        private readonly Dictionary<int, double> _capacityDictionary;
        private readonly CommutationTable _commutationTable;
        private readonly Dictionary<int, double> _freeCapacityDictionary;
        private readonly Random _random;
        private SubnetworkPointPool[] _subnetworkPointPools;

        private readonly Dictionary<UniqueId, SubnetworkPoint> _subnetworkPoints =
            new Dictionary<UniqueId, SubnetworkPoint>();


        public LinkResourceManager(NetworkAddress networkAddress, CommutationTable commutationTable, int numberOfPorts,
            int capacity) : base(networkAddress, ControlPlaneElementType.LRM) {
            _random = new Random();
            _commutationTable = commutationTable;
            _capacityDictionary = new Dictionary<int, double>();
            _freeCapacityDictionary = new Dictionary<int, double>();

            for (var i = 1; i <= numberOfPorts; i++) {
                _capacityDictionary.Add(i, capacity);
                _freeCapacityDictionary.Add(i, capacity);
            }
        }


        public int[] GetNewLabels(int portNumber) {
            int VPI;
            int VCI;

            while (true) {
                VPI = _random.Next() % MaxLabelNumber;
                VCI = _random.Next() % MaxLabelNumber;
                Debug.WriteLine(VPI + " " + VCI);

                if (_commutationTable.FindRow(VPI, VCI, portNumber) == null)
                    break;
            }
            return new[] {VPI, VCI};
        }

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case OperationType.GetLabels:
                    Debug.WriteLine("GetLabels " + (int) message.Payload + " port.");
                    var labels = GetNewLabels((int) message.Payload);
                    Debug.WriteLine(_capacityDictionary.Count + " Send labels " + (int) message.Payload + " port, VPI: " +
                                    labels[0] + ", VCI:" + labels[1]);
                    SendLabels(labels);
                    break;
                case OperationType.LinkConnectionRequest:
                    HandleLinkConnectionRequest(message);
                    break;
                case OperationType.SNPNegotiation:
                    HandleSnpNegotiation(message);
                    break;
                case OperationType.SNPNegotiationResponse:
                    HandleSnpNegotiationResponse(message);
                    break;
                case OperationType.SNPRelease:
                    HandleSnpRelease(message);
                    break;
                case OperationType.Confirm:
                    HandleConfirm(message);
                    break;
            }
        }


        private void HandleLinkConnectionRequest(SignallingMessage message) {
            message.Operation = OperationType.SNPNegotiation;
            _subnetworkPointPools = message.Payload as SubnetworkPointPool[];
            message.DestinationAddress = _subnetworkPointPools[1].NetworkAddress;
            message.DestinationControlPlaneElement = ControlPlaneElementType.LRM;
            message.Payload = _subnetworkPoints;
            SendMessage(message);
        }

        private void HandleSnpNegotiation(SignallingMessage message) {
            var collapse = true;
            SubnetworkPoint subnetworkPoint = null;
            var _subnetworkPointsReceived = message.Payload as Dictionary<UniqueId, SubnetworkPoint>;
            while (collapse) {
                subnetworkPoint = SubnetworkPoint.GenerateRandom(message.DemandedCapacity);
                if (!_subnetworkPointsReceived.ContainsValue(subnetworkPoint) &&
                    !_subnetworkPoints.ContainsValue(subnetworkPoint)) {
                    var myMsg = new SignallingMessage();
                    myMsg = message;
                    myMsg.Payload = subnetworkPoint;
                    myMsg.DestinationControlPlaneElement = ControlPlaneElementType.CC;
                    myMsg.DestinationAddress = myMsg.DestinationAddress.GetParentsAddress();
                    myMsg.Operation = OperationType.SetSNP;
                    SendMessage(myMsg);
                    message.Payload = subnetworkPoint;
                    message.Operation = OperationType.SNPNegotiationResponse;
                    message.DestinationAddress = message.SourceAddress;
                    message.DestinationControlPlaneElement =
                        ControlPlaneElementType.LRM;
                    _subnetworkPoints.Add(message.SessionId, subnetworkPoint);
                    _subnetworkPointPools[0].ReserveCapacity(subnetworkPoint.Capacity);
                    //_subnetworkPoints.Add(subnetworkPoint);
                    SendMessage(message);
                    collapse = false;
                }
            }
        }

        private void HandleSnpNegotiationResponse(SignallingMessage message) {
            var subnetworkPoint = message.Payload as SubnetworkPoint;
            _subnetworkPoints.Add(message.SessionId, subnetworkPoint);
            _subnetworkPointPools[1].ReserveCapacity(subnetworkPoint.Capacity);
            message.Payload = _subnetworkPointPools;
            message.DestinationControlPlaneElement = ControlPlaneElementType.RC;
            message.DestinationAddress = message.SourceAddress.GetParentsAddress();
            message.Operation = OperationType.LocalTopology;
            SendMessage(message);
            message.Operation = OperationType.LinkConnectionResponse;
            message.Payload = _subnetworkPoints[message.SessionId];
            message.DestinationControlPlaneElement = ControlPlaneElementType.CC;
            message.DestinationAddress = _subnetworkPointPools[0].NetworkNodeAddress;
            SendMessage(message);
        }

        private void HandleSnpRelease(SignallingMessage message) {
            var subnetworkPoint = message.Payload as SubnetworkPoint;

            //message.DestinationAddress = message.SourceAddress;
            //message.DestinationControlPlaneElement= SignallingMessageControlPlaneElement.LRM;
            //message.Operation = SignallingMessageOperation.Confirm;
            //message.Payload = _subnetworkPointPool;
            //SendMessage(message);
        }

        private void HandleConfirm(SignallingMessage message) {
            var subnetworkPointPool = message.Payload as SubnetworkPointPool;
            message.Payload =
                message.DestinationControlPlaneElement = ControlPlaneElementType.CC;
            message.DestinationAddress = message.SourceAddress.GetParentsAddress();
            //message.Operation = SignallingMessageOperation.Lin
        }

        private void SendLabels(int[] labels) {
            //SendMessage(new SignallingMessage(SignallingMessageOperation.SetLabels, labels));
        }
    }
}