using System;

namespace NetworkUtilities.ControlPlane.GraphAlgorithm {
    public class Link {
        public Link(SubnetworkPointPool beginSubnetworkPointPool, SubnetworkPointPool endSubnetworkPointPool) {
            if (beginSubnetworkPointPool.CapacityLeft != endSubnetworkPointPool.CapacityLeft)
                throw new ArgumentException("Link's SubnetworkPointPools are not compatible.");

            BeginSubnetworkPointPool = beginSubnetworkPointPool;
            EndSubnetworkPointPool = endSubnetworkPointPool;
            CapacityLeft = BeginSubnetworkPointPool.CapacityLeft;
        }

        public SubnetworkPointPool BeginSubnetworkPointPool { get; }
        public SubnetworkPointPool EndSubnetworkPointPool { get; private set; }
        public int CapacityLeft { get; private set; }
    }
}