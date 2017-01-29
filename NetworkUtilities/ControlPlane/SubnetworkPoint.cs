using System;

namespace NetworkUtilities.ControlPlane {
    [Serializable]
    public class SubnetworkPoint {
        private static readonly Random Random = new Random();

        public int Vpi { get; private set; }
        public int Vci { get; private set; }
        public int Capacity { get; private set; }

        private SubnetworkPoint(int vpi, int vci, int capacity) {
            Vpi = vpi;
            Vci = vci;
            Capacity = capacity;
        }

        public static SubnetworkPoint GenerateRandom(int capacity) {
            var vpi = Random.Next(4096);
            var vci = Random.Next(65536);

            return new SubnetworkPoint(vpi, vci, capacity);
        }
    }
}