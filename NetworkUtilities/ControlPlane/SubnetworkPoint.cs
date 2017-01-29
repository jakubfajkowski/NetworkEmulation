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

        protected bool Equals(SubnetworkPoint other) {
            return Vpi == other.Vpi && Vci == other.Vci;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubnetworkPoint) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Vpi*397) ^ Vci;
            }
        }

        public static bool operator ==(SubnetworkPoint left, SubnetworkPoint right) {
            return Equals(left, right);
        }

        public static bool operator !=(SubnetworkPoint left, SubnetworkPoint right) {
            return !Equals(left, right);
        }
    }
}