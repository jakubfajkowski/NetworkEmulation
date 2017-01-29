namespace NetworkUtilities.GraphAlgorithm {
    public class Path {
        private readonly SubnetworkPointPool[] _subnetworkPointPools;

        public Path(int n) {
            _subnetworkPointPools = new SubnetworkPointPool[n];
            Length = 0;
            SumWeight = 0;
            MinWeight = double.MaxValue;
        }

        public SubnetworkPointPool[] SubnetworkPointPools {
            get {
                var existingSubnetworkPointPools = new SubnetworkPointPool[Length];
                for (var i = 0; i < Length; i++)
                    existingSubnetworkPointPools[i] = _subnetworkPointPools[Length - 1 - i];
                return existingSubnetworkPointPools;
            }
        }

        public double MinWeight { get; }

        public double SumWeight { get; }

        public int Length { get; private set; }

        public void push(SubnetworkPointPool subnetworkPointPool) {
            _subnetworkPointPools[Length++] = subnetworkPointPool;
        }
    }
}