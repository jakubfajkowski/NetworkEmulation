using System;
using System.Collections.Generic;

namespace NetworkUtilities.ControlPlane {
    [Serializable]
    public class SubnetworkPointPool {
        public NetworkAddress NetworkAddress { get; }
        public int CapacityLeft { get; private set; }

        public NetworkAddress NetworkNodeAddress => NetworkAddress.GetParentsAddress();
        public int Id => NetworkAddress.GetLastId();

        public SubnetworkPointPool(NetworkAddress networkAddress) {
            NetworkAddress = networkAddress;
        }

        public bool ReserveCapacity(int demandedCapacity) {
            if (CapacityLeft >= demandedCapacity) {
                CapacityLeft -= demandedCapacity;
                return true;
            }

            return false;
        }

        public void ReleaseCapacity(int capacityToRelease) {
            CapacityLeft += capacityToRelease;
        }
    }
}
