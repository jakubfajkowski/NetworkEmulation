using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.GraphAlgorithm;

namespace NetworkNode {
    public class LinkResourceManager : ControlPlaneElement {
        public static int MaxLabelNumber = 1000;
        private readonly Dictionary<int, double> _capacityDictionary;
        private readonly CommutationTable _commutationTable;
        private readonly Dictionary<int, double> _freeCapacityDictionary;
        private readonly Random _random;
        private Dictionary<UniqueId,SubnetworkPoint> _subnetworkPoints = new Dictionary<UniqueId, SubnetworkPoint>();
        private SubnetworkPointPool[] _subnetworkPointPools;


        public LinkResourceManager(NetworkAddress networkAddress, CommutationTable commutationTable, int numberOfPorts,
            int capacity) : base(networkAddress) {
            _random = new Random();
            this._commutationTable = commutationTable;
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
                VPI = _random.Next()%MaxLabelNumber;
                VCI = _random.Next()%MaxLabelNumber;
                Debug.WriteLine(VPI + " " + VCI);

                if (_commutationTable.FindRow(VPI, VCI, portNumber) == null)
                    break;
            }
            return new[] {VPI, VCI};
        }

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.GetLabels:
                    Debug.WriteLine("GetLabels " + (int) message.Payload + " port.");
                    var labels = GetNewLabels((int) message.Payload);
                    Debug.WriteLine(_capacityDictionary.Count + " Send labels " + (int) message.Payload + " port, VPI: " +
                                    labels[0] + ", VCI:" + labels[1]);
                    SendLabels(labels);
                    break;
                case SignallingMessageOperation.LinkConnectionRequest:
                    HandleLinkConnectionRequest(message);
                    break;
                case SignallingMessageOperation.SNPNegotiation:
                    HandleSnpNegotiation(message);
                    break;
                case SignallingMessageOperation.SNPNegotiationResponse:
                    HandleSnpNegotiationResponse(message);
                    break;
                case SignallingMessageOperation.SNPRelease:
                    HandleSnpRelease(message);
                    break;
                case SignallingMessageOperation.Confirm:
                    HandleConfirm(message);
                    break;
            }
        }
       

        private void HandleLinkConnectionRequest(SignallingMessage message) {
            message.Operation = SignallingMessageOperation.SNPNegotiation;
            _subnetworkPointPools = message.Payload as SubnetworkPointPool[];
            message.DestinationAddress = _subnetworkPointPools[1].NetworkAddress;
            message.DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.LinkResourceManager;
            message.Payload = _subnetworkPoints;
            SendMessage(message);
        }

        private void HandleSnpNegotiation(SignallingMessage message) {
            var collapse = true;
            SubnetworkPoint subnetworkPoint = null;
            var _subnetworkPointsReceived = message.Payload as Dictionary<UniqueId,SubnetworkPoint>;
            while (collapse) {
                subnetworkPoint = SubnetworkPoint.GenerateRandom(message.DemandedCapacity);
                if (!_subnetworkPointsReceived.ContainsValue(subnetworkPoint) && !_subnetworkPoints.ContainsValue(subnetworkPoint)) {
                    var myMsg = new SignallingMessage();
                    myMsg = message;
                    myMsg.Payload = subnetworkPoint;
                    myMsg.DestinationControlPlaneElement= SignallingMessageDestinationControlPlaneElement.ConnectionController;
                    myMsg.DestinationAddress = myMsg.DestinationAddress.GetParentsAddress();
                    myMsg.Operation= SignallingMessageOperation.SetSNP;
                    SendMessage(myMsg);
                    message.Payload = subnetworkPoint;
                    message.Operation = SignallingMessageOperation.SNPNegotiationResponse;
                    message.DestinationAddress = message.SourceAddress;
                    message.DestinationControlPlaneElement =
                        SignallingMessageDestinationControlPlaneElement.LinkResourceManager;
                    _subnetworkPoints.Add(message.SessionId,subnetworkPoint);
                    _subnetworkPointPools[0].ReserveCapacity(subnetworkPoint.Capacity);
                    //_subnetworkPoints.Add(subnetworkPoint);
                    SendMessage(message);
                    collapse = false;
                }
            }
        }

        private void HandleSnpNegotiationResponse(SignallingMessage message) {
            var subnetworkPoint= message.Payload as SubnetworkPoint;
            _subnetworkPoints.Add(message.SessionId, subnetworkPoint);
            _subnetworkPointPools[1].ReserveCapacity(subnetworkPoint.Capacity);
            message.Payload = _subnetworkPointPools;
            message.DestinationControlPlaneElement= SignallingMessageDestinationControlPlaneElement.RoutingController;
            message.DestinationAddress = message.SourceAddress.GetParentsAddress();
            message.Operation = SignallingMessageOperation.LocalTopology;
            SendMessage(message);
            message.Operation = SignallingMessageOperation.LinkConnectionResponse;
            message.Payload = _subnetworkPoints[message.SessionId];
            message.DestinationControlPlaneElement= SignallingMessageDestinationControlPlaneElement.ConnectionController;
            message.DestinationAddress = _subnetworkPointPools[0].NetworkNodeAddress;
            SendMessage(message);
        }

        private void HandleSnpRelease(SignallingMessage message) {
            var subnetworkPoint = message.Payload as SubnetworkPoint;
           
            //message.DestinationAddress = message.SourceAddress;
            //message.DestinationControlPlaneElement= SignallingMessageDestinationControlPlaneElement.LinkResourceManager;
            //message.Operation = SignallingMessageOperation.Confirm;
            //message.Payload = _subnetworkPointPool;
            //SendMessage(message);
        }

        private void HandleConfirm(SignallingMessage message) {
            var subnetworkPointPool = message.Payload as SubnetworkPointPool;
            message.Payload = 
            message.DestinationControlPlaneElement= SignallingMessageDestinationControlPlaneElement.ConnectionController;
            message.DestinationAddress = message.SourceAddress.GetParentsAddress();
            //message.Operation = SignallingMessageOperation.Lin
        }
        private void SendLabels(int[] labels) {
            //SendMessage(new SignallingMessage(SignallingMessageOperation.SetLabels, labels));
        }
        
    }
}