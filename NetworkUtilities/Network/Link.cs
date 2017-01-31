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

        public static Link Reverse(Link link) {
            return new Link(link.EndSubnetworkPointPool, link.BeginSubnetworkPointPool);
        }

        public override string ToString() {
            return $"{BeginSubnetworkPointPool}->{EndSubnetworkPointPool}, Capacity: {CapacityLeft}";
        }
    }
}