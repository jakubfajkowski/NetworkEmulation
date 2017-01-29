using System;
using System.Collections.Generic;

namespace NetworkUtilities.GraphAlgorithm {
    internal class Graph {
        public Vertex[] Vertices { get; private set; }

        public Edge[] Edges { get; private set; }


        private string getDataFromLine(string s, int n) {
            string[] stringSeparator = {" = ", " "};

            return s.Split(stringSeparator, StringSplitOptions.None)[n];
        }


        public void load(List<string> textFile) {
            Vertices = new Vertex[int.Parse(getDataFromLine(textFile[0], 1))];
            if (Vertices.Length == 0) throw new Exception("Zerowa liczba wierzchołków!");
            for (var i = 0; i < Vertices.Length; i++)
                Vertices[i] = new Vertex(i + 1);

            Edges = new Edge[int.Parse(getDataFromLine(textFile[1], 1))];
            if (Edges.Length == 0) throw new Exception("Zerowa liczba krawędzi!");
            for (var i = 0; i < Edges.Length; i++) {
                var edge_id = int.Parse(getDataFromLine(textFile[2 + i], 0));
                var begin_id = int.Parse(getDataFromLine(textFile[2 + i], 1));
                var end_id = int.Parse(getDataFromLine(textFile[2 + i], 2));

                Edges[i] = new Edge(edge_id, Vertices[begin_id - 1], Vertices[end_id - 1]);
                Edges[i].Begin.addEdgeOut(Edges[i]);
            }
        }

        public void randomizeEdgesWeights() {
            var generator = new Random();
            for (var i = 0; i < Edges.Length; i++) {
                var randomWeight = generator.NextDouble();
                Edges[i].Weight = randomWeight;
            }
        }
    }
}