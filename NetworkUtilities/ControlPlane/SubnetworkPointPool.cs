using System;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    [Serializable]
    public class SubnetworkPointPool {
        public SubnetworkPointPool(NetworkAddress networkAddress) {
            NetworkAddress = networkAddress;
        }

        public SubnetworkPointPool(NetworkAddressNodePortPair pair) :
            this(pair.NetworkAddress.Append(pair.NodePortNumber)) {}

        public NetworkAddress NetworkAddress { get; }

        public NetworkAddress NodeAddress => NetworkAddress.GetParentsAddress();
        public int Id => NetworkAddress.GetLastId();

        protected bool Equals(SubnetworkPointPool other) {
            return Equals(NetworkAddress, other.NetworkAddress);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SubnetworkPointPool) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((NetworkAddress?.GetHashCode() ?? 0) * 397);
            }
        }

        public override string ToString() {
            return $"{NodeAddress}:{Id}";
        }
    }
}