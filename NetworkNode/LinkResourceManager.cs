using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;

namespace NetworkNode {
    public class LinkResourceManager : ControlPlaneElement {
        public static int maxLabelNumber = 1000;
        private readonly Dictionary<int, double> capacityDictionary;
        private readonly CommutationTable commutationTable;
        private readonly Dictionary<int, double> freeCapacityDictionary;
        private readonly Random random;

        public LinkResourceManager(NetworkAddress networkAddress, CommutationTable commutationTable, int numberOfPorts,
            int capacity) : base(networkAddress) {
            random = new Random();
            this.commutationTable = commutationTable;
            capacityDictionary = new Dictionary<int, double>();
            freeCapacityDictionary = new Dictionary<int, double>();

            for (var i = 1; i <= numberOfPorts; i++) {
                capacityDictionary.Add(i, capacity);
                freeCapacityDictionary.Add(i, capacity);
            }
        }

        public int[] getNewLabels(int portNumber) {
            int VPI;
            int VCI;

            while (true) {
                VPI = random.Next() % maxLabelNumber;
                VCI = random.Next() % maxLabelNumber;
                Debug.WriteLine(VPI + " " + VCI);

                if (commutationTable.FindRow(VPI, VCI, portNumber) == null)
                    break;
            }
            return new[] {VPI, VCI};
        }

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.GetLabels:
                    Debug.WriteLine("GetLabels " + (int) message.Payload + " port.");
                    var labels = getNewLabels((int) message.Payload);
                    Debug.WriteLine(capacityDictionary.Count + " Send labels " + (int) message.Payload + " port, VPI: " +
                                    labels[0] + ", VCI:" + labels[1]);
                    sendLabels(labels);
                    break;
            }
        }

        private void sendLabels(int[] labels) {
            //SendMessage(new SignallingMessage(SignallingMessageOperation.SetLabels, labels));
        }
    }
}