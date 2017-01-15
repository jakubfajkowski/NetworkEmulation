using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.ControlPlane;

namespace NetworkNode
{
    public class LinkResourceManager : ControlPlaneElement
    {
        public static int maxLabelNumber = 1000;
        private CommutationTable commutationTable;
        private Dictionary<int, double> capacityDictionary;
        private Dictionary<int, double> freeCapacityDictionary;
        private Random random;

        public LinkResourceManager(CommutationTable commutationTable, int numberOfPorts, int capacity)
        {
            random = new Random();
            this.commutationTable = commutationTable;
            capacityDictionary = new Dictionary<int, double>();
            freeCapacityDictionary = new Dictionary<int, double>();

            for (int i = 1; i <= numberOfPorts; i++)
            {
                capacityDictionary.Add(i, capacity);
                freeCapacityDictionary.Add(i, capacity);
            }
        }

        public int[] getNewLabels(int portNumber)
        {
            int VPI;
            int VCI;

            while (true)
            {
                VPI = random.Next() % maxLabelNumber;
                VCI = random.Next() % maxLabelNumber;
                Debug.WriteLine(VPI + " " + VCI);

                if (commutationTable.FindRow(VPI, VCI, portNumber) == null)
                    break;
            }
            return new int[] { VPI, VCI };
        }

        public override void ReceiveMessage(SignallingMessage message)
        {
            switch (message.Operation)
            {
                case SignallingMessageOperation.GetLabels:
                    Debug.WriteLine("GetLabels " + (int)message.Payload + " port.");
                    int[] labels = getNewLabels((int)message.Payload);
                    Debug.WriteLine(capacityDictionary.Count+" Send labels " + (int)message.Payload + " port, VPI: " + labels[0] + ", VCI:" + labels[1]);
                    sendLabels(labels);
                    break;
            }
        }

        private void sendLabels(int[] labels)
        {
            //SendMessage(new SignallingMessage(SignallingMessageOperation.SetLabels, labels));
        }


    }
}
