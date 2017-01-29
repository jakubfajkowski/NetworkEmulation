namespace NetworkUtilities.GraphAlgorithm {
    internal class Edge {
        public Edge(int id, Vertex begin, Vertex end) {
            Id = id;
            Begin = begin;
            End = end;
        }

        public int Id { get; set; }

        public Vertex Begin { get; set; }

        public Vertex End { get; set; }

        public double Weight { get; set; }
    }
}