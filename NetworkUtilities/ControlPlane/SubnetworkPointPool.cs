using System;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    [Serializable]
    public class SubnetworkPointPool {
        public SubnetworkPointPool(NetworkAddress networkAddress) {
            NetworkAddress = networkAddress;
        }

        public NetworkAddress NetworkAddress { get; }
        public int CapacityLeft { get; private set; }

        public NetworkAddress NetworkNodeAddress => NetworkAddress.GetParentsAddress();
        public int Id => NetworkAddress.GetLastId();

        public void ReserveCapacity(int demandedCapacity) {
            CapacityLeft -= demandedCapacity;
        }

        public void ReleaseCapacity(int releasedCapacity) {
            CapacityLeft += releasedCapacity;
        }

        protected bool Equals(SubnetworkPointPool other) {
            return Equals(NetworkAddress, other.NetworkAddress) && CapacityLeft == other.CapacityLeft;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SubnetworkPointPool) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((NetworkAddress?.GetHashCode() ?? 0) * 397) ^ CapacityLeft;
            }
        }

        public override string ToString() {
            return $"{NetworkNodeAddress}:{Id}";
        }
    }
}