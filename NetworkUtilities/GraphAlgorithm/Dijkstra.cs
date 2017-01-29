using System;
using NetworkUtilities.GraphAlgorithm.PriorityQueue;

namespace NetworkUtilities.GraphAlgorithm {
    internal static class Dijkstra {
        private const double infinity = double.MaxValue;
        private static Graph graph;

        public static Path[] runAlgorithm(Graph graph_, Vertex begin) {
            graph = graph_;
            var widestPaths = new Path[graph.Vertices.Length];
            var verticesHeap = new Heap<Vertex>();

            initialize(begin);
            verticesHeap.initialise(graph.Vertices.Length);
            for (var i = 0; i < graph.Vertices.Length; i++)
                verticesHeap.insertElement(new Element<Vertex>(graph.Vertices[i].CumulatedWeight,
                    graph.Vertices[i]));

            while (verticesHeap.NumberOfElements != 0) {
                var currentVertex = verticesHeap.deleteMax().Data;
                if (currentVertex.CumulatedWeight == 0)
                    break;

                foreach (var e in currentVertex.EdgesOut) {
                    var neighbor = e.End;

                    var alternate = Math.Max(neighbor.CumulatedWeight, Math.Min(currentVertex.CumulatedWeight, e.Weight));

                    if (alternate > neighbor.CumulatedWeight) {
                        neighbor.CumulatedWeight = alternate;
                        neighbor.Prev = currentVertex;
                    }
                }
                sortHeap(ref verticesHeap);
            }

            for (var i = 0; i < graph.Vertices.Length; i++)
                widestPaths[i] = generatePath(begin, graph.Vertices[i]);

            return widestPaths;
        }


        public static Path[] runAlgorithm(Graph graph_, Vertex begin, Vertex end) {
            graph = graph_;
            var widestPath = new Path[graph.Vertices.Length];
            var verticesHeap = new Heap<Vertex>();

            initialize(begin);
            verticesHeap.initialise(graph.Vertices.Length);
            for (var i = 0; i < graph.Vertices.Length; i++)
                verticesHeap.insertElement(new Element<Vertex>(graph.Vertices[i].CumulatedWeight,
                    graph.Vertices[i]));

            while (verticesHeap.NumberOfElements != 0) {
                var currentVertex = verticesHeap.deleteMax().Data;
                if (currentVertex.CumulatedWeight == 0)
                    break;


                foreach (var e in currentVertex.EdgesOut) {
                    var neighbor = e.End;


                    var alternate = Math.Max(neighbor.CumulatedWeight, Math.Min(currentVertex.CumulatedWeight, e.Weight));

                    if (alternate > neighbor.CumulatedWeight) {
                        neighbor.CumulatedWeight = alternate;
                        neighbor.Prev = currentVertex;
                    }
                }


                sortHeap(ref verticesHeap);
            }


            widestPath[0] = generatePath(begin, end);


            return widestPath;
        }

        public static Path[,] runAlgorithm(Graph graph_) {
            graph = graph_;
            var widestPaths = new Path[graph.Vertices.Length, graph.Vertices.Length];
            var verticesHeap = new Heap<Vertex>();


            for (var b = 0; b < graph.Vertices.Length; b++) {
                var begin = graph.Vertices[b];
                initialize(begin);
                verticesHeap.initialise(graph.Vertices.Length);
                for (var i = 0; i < graph.Vertices.Length; i++)
                    verticesHeap.insertElement(new Element<Vertex>(graph.Vertices[i].CumulatedWeight,
                        graph.Vertices[i]));

                while (verticesHeap.NumberOfElements != 0) {
                    var currentVertex = verticesHeap.deleteMax().Data;
                    if (currentVertex.CumulatedWeight == 0)
                        break;


                    foreach (var e in currentVertex.EdgesOut) {
                        var neighbor = e.End;


                        var alternate = Math.Max(neighbor.CumulatedWeight,
                            Math.Min(currentVertex.CumulatedWeight, e.Weight));

                        if (alternate > neighbor.CumulatedWeight) {
                            neighbor.CumulatedWeight = alternate;
                            neighbor.Prev = currentVertex;
                        }
                    }


                    sortHeap(ref verticesHeap);
                }

                for (var i = 0; i < graph.Vertices.Length; i++)
                    widestPaths[b, i] = generatePath(begin, graph.Vertices[i]);
            }
            return widestPaths;
        }


        private static void initialize(Vertex begin) {
            for (var i = 0; i < graph.Vertices.Length; i++) {
                graph.Vertices[i].CumulatedWeight = 0;
                graph.Vertices[i].Prev = null;
            }

            graph.Vertices[begin.Id - 1].CumulatedWeight = infinity;

            begin.Prev = begin;
        }

        private static void sortHeap(ref Heap<Vertex> heap) {
            var updatedKeys = new Element<Vertex>[heap.NumberOfElements];
            for (var i = 0; i < updatedKeys.Length; i++)
                updatedKeys[i] = updateKey(heap.deleteMax());
            for (var i = 0; i < updatedKeys.Length; i++)
                heap.insertElement(updatedKeys[i]);
        }

        private static Element<Vertex> updateKey(Element<Vertex> element) {
            element.Key = element.Data.CumulatedWeight;
            return element;
        }

        private static Path generatePath(Vertex begin, Vertex end) {
            var path = new Path(graph.Vertices.Length);
            var currentVertex = end;

            while (currentVertex != begin) {
                if (currentVertex == null)
                    return null;
                path.push(currentVertex);
                currentVertex = currentVertex.Prev;
            }
            path.push(begin);

            return path;
        }
    }
}