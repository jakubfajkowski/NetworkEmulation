using System;
using NetworkUtilities.ControlPlane;

namespace NetworkUtilities.Network {
    [Serializable]
    public class Link {
        public Link(SubnetworkPointPool beginSubnetworkPointPool, SubnetworkPointPool endSubnetworkPointPool) {
            if (beginSubnetworkPointPool.CapacityLeft != endSubnetworkPointPool.CapacityLeft)
                throw new ArgumentException("Link's SubnetworkPointPools are not compatible.");

            BeginSubnetworkPointPool = beginSubnetworkPointPool;
            EndSubnetworkPointPool = endSubnetworkPointPool;
            CapacityLeft = BeginSubnetworkPointPool.CapacityLeft;
        }

        public Link(LinkModel model) :
            this(new SubnetworkPointPool(model.InputNodePortPair, model.Capacity), 
                new SubnetworkPointPool(model.OutputNodePortPair, model.Capacity)) {}

        public SubnetworkPointPool BeginSubnetworkPointPool { get; }
        public SubnetworkPointPool EndSubnetworkPointPool { get; private set; }
        public int CapacityLeft { get; private set; }

        public Link Reverse() {
            return new Link(EndSubnetworkPointPool, BeginSubnetworkPointPool);
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
            return $"{BeginSubnetworkPointPool}->{EndSubnetworkPointPool}, Capacity: {CapacityLeft}";
        }
    }
}