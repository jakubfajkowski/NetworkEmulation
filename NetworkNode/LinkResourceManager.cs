using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.ControlPlane;

namespace NetworkNode
{
    class LinkResourceManager : ControlPlaneElement
    {

        public static int maxLabelNumber = 1000;
        private readonly CommutationTable commutationTable;
        private Dictionary<int, double> capacityDictionary;
        private Dictionary<int, double> freeCapacityDictionary;

        Random random;

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

        public void getNewLabels(int portNumber)
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

            // ZWRACA VPI VCI
        }

        public override void RecieveMessage(SignallingMessage message)
        {

        }

    }
}
