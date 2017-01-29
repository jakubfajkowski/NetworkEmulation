using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;

namespace NetworkNode {
    public class LinkResourceManager : ControlPlaneElement {
        public static int MaxLabelNumber = 1000;
        private readonly Dictionary<int, double> _capacityDictionary;
        private readonly CommutationTable _commutationTable;
        private readonly Dictionary<int, double> _freeCapacityDictionary;
        private readonly Random _random;

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
            }
        }

        private void SendLabels(int[] labels) {
            //SendMessage(new SignallingMessage(SignallingMessageOperation.SetLabels, labels));
        }
    }
}