namespace NetworkUtilities.GraphAlgorithm {
    internal class Vertex {
        public Vertex() {
            Id = 0;
            EdgesOut = new Edge[0];
        }

        public Vertex(int id) {
            Id = id;
            EdgesOut = new Edge[0];
        }

        public int Id { get; set; }

        public Vertex Prev { get; set; }

        public double CumulatedWeight { get; set; }

        public Edge[] EdgesOut { get; private set; }

        public void addEdgeOut(Edge edge) {
            var tmp_links = new Edge[EdgesOut.Length + 1];
            for (var i = 0; i < EdgesOut.Length; i++)
                tmp_links[i] = EdgesOut[i];
            tmp_links[EdgesOut.Length] = edge;
            EdgesOut = tmp_links;
        }
    }
}