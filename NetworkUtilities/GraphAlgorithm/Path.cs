namespace NetworkUtilities.GraphAlgorithm {
    internal class Path {
        private readonly Vertex[] vertices;

        public Path(int n) {
            vertices = new Vertex[n];
            Length = 0;
            SumWeight = 0;
            MinWeight = double.MaxValue;
        }

        public Vertex[] Vertices {
            get {
                var existingVertices = new Vertex[Length];
                for (var i = 0; i < Length; i++)
                    existingVertices[i] = vertices[Length - 1 - i];
                return existingVertices;
            }
        }

        public double MinWeight { get; }

        public double SumWeight { get; }

        public int Length { get; private set; }

        public void push(Vertex vertex) {
            vertices[Length++] = vertex;
        }
    }
}