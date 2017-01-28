namespace NetworkUtilities.GraphAlgorithm {
    public class Link {
        public int Id { get; set; }

        public SubnetworkPointPool Begin { get; set; }

        public SubnetworkPointPool End { get; set; }

        public double Weight { get; } = 1.0;

        public static int LinkCounter { get; private set; }

        public int Capacity { get; }

        public Link(int capacity,SubnetworkPointPool begin, SubnetworkPointPool end) {
            LinkCounter++;
            Id = LinkCounter;
            Begin = begin;
            End = end;
            Capacity = capacity;
        }
    }
}