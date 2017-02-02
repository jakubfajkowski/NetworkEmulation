using System;
using NetworkUtilities.ControlPlane;

namespace NetworkUtilities.Network {
    [Serializable]
    public class Link {
        public bool IsClientLink { get; }

        public Link(SubnetworkPointPool beginSubnetworkPointPool, SubnetworkPointPool endSubnetworkPointPool, int capacity, bool isClientLink) {
            BeginSubnetworkPointPool = beginSubnetworkPointPool;
            EndSubnetworkPointPool = endSubnetworkPointPool;
            CapacityLeft = capacity;
        }

        public Link(LinkModel model, bool isClientLink) : 
            this(new SubnetworkPointPool(model.InputNodePortPair), new SubnetworkPointPool(model.OutputNodePortPair), model.Capacity, isClientLink) {
            IsClientLink = isClientLink;
        }

        public SubnetworkPointPool BeginSubnetworkPointPool { get; }
        public SubnetworkPointPool EndSubnetworkPointPool { get; private set; }
        public int CapacityLeft { get; private set; }

        public void ReserveCapacity(int demandedCapacity) {
            CapacityLeft -= demandedCapacity;
        }

        public void ReleaseCapacity(int releasedCapacity) {
            CapacityLeft += releasedCapacity;
        }

        public Link Reverse() {
            return new Link(EndSubnetworkPointPool, BeginSubnetworkPointPool, CapacityLeft, IsClientLink);
        }

        protected bool Equals(Link other) {
            return Equals(BeginSubnetworkPointPool, other.BeginSubnetworkPointPool) && Equals(EndSubnetworkPointPool, other.EndSubnetworkPointPool);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Link) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((BeginSubnetworkPointPool != null ? BeginSubnetworkPointPool.GetHashCode() : 0) * 397) ^ (EndSubnetworkPointPool != null ? EndSubnetworkPointPool.GetHashCode() : 0);
            }
        }

        public override string ToString() {
            return $"{BeginSubnetworkPointPool}->{EndSubnetworkPointPool}, Capacity: {CapacityLeft} {IsClientLink}";
        }
    }
}