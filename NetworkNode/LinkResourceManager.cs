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
        private List<SubnetworkPoint> _subnetworkPoints = new List<SubnetworkPoint>();
        private SubnetworkPointPool _subnetworkPointPool;


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
            }
        }

        

        public event CommutationTableRecordHandler OnClientTableRowAdded;
        public event CommutationTableRecordHandler OnClientTableRowDeleted;

        private void HandleLinkConnectionRequest(SignallingMessage message) {
            message.Operation = SignallingMessageOperation.SNPNegotiation;
             var snpps=  message.Payload as SubnetworkPointPool[];
            message.DestinationAddress = snpps[1].NetworkAddress;
            message.Payload = SubnetworkPoint.GenerateRandom(message.DemandedCapacity);
            SendMessage(message);
            }
        private void SendLabels(int[] labels) {
            //SendMessage(new SignallingMessage(SignallingMessageOperation.SetLabels, labels));
        }

        protected virtual void OnOnClientTableRowAdded(CommutationTableRecordHandlerArgs args) {
            OnClientTableRowAdded?.Invoke(this, args);
        }

        protected virtual void OnOnClientTableRowDeleted(CommutationTableRecordHandlerArgs args) {
            OnClientTableRowDeleted?.Invoke(this, args);
        }
    }

    public delegate void CommutationTableRecordHandler(object sender, CommutationTableRecordHandlerArgs args);

    public class CommutationTableRecordHandlerArgs {
        public int InVpi { get; private set; }
        public int InVci { get; private set; }
        public int InPortNumber { get; private set; }
        public int OutVpi { get; private set; }
        public int OutVci { get; private set; }
        public int LinkNumber { get; private set; }

        public CommutationTableRecordHandlerArgs(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci, int linkNumber) {
            InVpi = inVpi;
            InVci = inVci;
            InPortNumber = inPortNumber;
            OutVpi = outVpi;
            OutVci = outVci;
            LinkNumber = linkNumber;
        }
    }
}