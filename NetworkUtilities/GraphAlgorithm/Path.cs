namespace NetworkUtilities.GraphAlgorithm
{
    public class Path
    {
        private SubnetworkPointPool[] _subnetworkPointPools;
        public SubnetworkPointPool[] SubnetworkPointPools
        {
            get
            {
                SubnetworkPointPool[] existingSubnetworkPointPools = new SubnetworkPointPool[length];
                for (int i = 0; i < length; i++)
                {
                    existingSubnetworkPointPools[i] = _subnetworkPointPools[length - 1 - i];
                }
                return existingSubnetworkPointPools;
            }
        }
 
        private double minWeight;
        public double MinWeight
        {
            get { return minWeight; }
        }

        private double sumWeight;
        public double SumWeight
        {
            get { return sumWeight; }
        }

        private int length;
        public int Length
        {
            get { return length; }
        }

        public void push(SubnetworkPointPool subnetworkPointPool)
        {
            _subnetworkPointPools[length++] = subnetworkPointPool;
        }
        public Path(int n)
        {
            _subnetworkPointPools = new SubnetworkPointPool[n];
            length = 0;
            sumWeight = 0;
            minWeight = double.MaxValue;
        }
    }
}
