using System;
using NetworkUtilities.GraphAlgorithm.PriorityQueue;

namespace NetworkUtilities.GraphAlgorithm {
    public static class Dijkstra {
        private const double infinity = double.MaxValue;
        private static Graph graph;

        public static Path[] runAlgorithm(Graph graph_, SubnetworkPointPool begin) {
            graph = graph_;
            var widestPaths = new Path[graph.SubnetworkPointPools.Length];
            var verticesHeap = new Heap<SubnetworkPointPool>();

            initialize(begin);
            verticesHeap.initialise(graph.SubnetworkPointPools.Length);
            for (var i = 0; i < graph.SubnetworkPointPools.Length; i++)
                verticesHeap.insertElement(
                    new Element<SubnetworkPointPool>(graph.SubnetworkPointPools[i].CumulatedWeight,
                        graph.SubnetworkPointPools[i]));

            while (verticesHeap.NumberOfElements != 0) {
                var currentSubnetworkPointPool = verticesHeap.deleteMax().Data;
                if (currentSubnetworkPointPool.CumulatedWeight == 0)
                    break;

                foreach (var e in currentSubnetworkPointPool.LinksOut) {
                    var neighbor = e.End;

                    var alternate = Math.Max(neighbor.CumulatedWeight,
                        Math.Min(currentSubnetworkPointPool.CumulatedWeight, e.Weight));

                    if (alternate > neighbor.CumulatedWeight) {
                        neighbor.CumulatedWeight = alternate;
                        neighbor.Prev = currentSubnetworkPointPool;
                    }
                }
                sortHeap(ref verticesHeap);
            }

            for (var i = 0; i < graph.SubnetworkPointPools.Length; i++)
                widestPaths[i] = generatePath(begin, graph.SubnetworkPointPools[i]);

            return widestPaths;
        }


        public static Path[] runAlgorithm(Graph graph_, SubnetworkPointPool begin, SubnetworkPointPool end) {
            graph = graph_;
            var widestPath = new Path[graph.SubnetworkPointPools.Length];
            var verticesHeap = new Heap<SubnetworkPointPool>();

            initialize(begin);
            verticesHeap.initialise(graph.SubnetworkPointPools.Length);
            for (var i = 0; i < graph.SubnetworkPointPools.Length; i++)
                verticesHeap.insertElement(
                    new Element<SubnetworkPointPool>(graph.SubnetworkPointPools[i].CumulatedWeight,
                        graph.SubnetworkPointPools[i]));

            while (verticesHeap.NumberOfElements != 0) {
                var currentSubnetworkPointPool = verticesHeap.deleteMax().Data;
                if (currentSubnetworkPointPool.CumulatedWeight == 0)
                    break;


                foreach (var e in currentSubnetworkPointPool.LinksOut) {
                    var neighbor = e.End;


                    var alternate = Math.Max(neighbor.CumulatedWeight,
                        Math.Min(currentSubnetworkPointPool.CumulatedWeight, e.Weight));

                    if (alternate > neighbor.CumulatedWeight) {
                        neighbor.CumulatedWeight = alternate;
                        neighbor.Prev = currentSubnetworkPointPool;
                    }
                }


                sortHeap(ref verticesHeap);
            }


            widestPath[0] = generatePath(begin, end);


            return widestPath;
        }

        public static Path[,] runAlgorithm(Graph graph_) {
            graph = graph_;
            var widestPaths = new Path[graph.SubnetworkPointPools.Length, graph.SubnetworkPointPools.Length];
            var verticesHeap = new Heap<SubnetworkPointPool>();


            for (var b = 0; b < graph.SubnetworkPointPools.Length; b++) {
                var begin = graph.SubnetworkPointPools[b];
                initialize(begin);
                verticesHeap.initialise(graph.SubnetworkPointPools.Length);
                for (var i = 0; i < graph.SubnetworkPointPools.Length; i++)
                    verticesHeap.insertElement(
                        new Element<SubnetworkPointPool>(graph.SubnetworkPointPools[i].CumulatedWeight,
                            graph.SubnetworkPointPools[i]));

                while (verticesHeap.NumberOfElements != 0) {
                    var currentSubnetworkPointPool = verticesHeap.deleteMax().Data;
                    if (currentSubnetworkPointPool.CumulatedWeight == 0)
                        break;


                    foreach (var e in currentSubnetworkPointPool.LinksOut) {
                        var neighbor = e.End;


                        var alternate = Math.Max(neighbor.CumulatedWeight,
                            Math.Min(currentSubnetworkPointPool.CumulatedWeight, e.Weight));

                        if (alternate > neighbor.CumulatedWeight) {
                            neighbor.CumulatedWeight = alternate;
                            neighbor.Prev = currentSubnetworkPointPool;
                        }
                    }


                    sortHeap(ref verticesHeap);
                }

                for (var i = 0; i < graph.SubnetworkPointPools.Length; i++)
                    widestPaths[b, i] = generatePath(begin, graph.SubnetworkPointPools[i]);
            }
            return widestPaths;
        }


        private static void initialize(SubnetworkPointPool begin) {
            for (var i = 0; i < graph.SubnetworkPointPools.Length; i++) {
                graph.SubnetworkPointPools[i].CumulatedWeight = 0;
                graph.SubnetworkPointPools[i].Prev = null;
            }

            graph.SubnetworkPointPools[begin.Id - 1].CumulatedWeight = infinity;

            begin.Prev = begin;
        }

        private static void sortHeap(ref Heap<SubnetworkPointPool> heap) {
            var updatedKeys = new Element<SubnetworkPointPool>[heap.NumberOfElements];
            for (var i = 0; i < updatedKeys.Length; i++)
                updatedKeys[i] = updateKey(heap.deleteMax());
            for (var i = 0; i < updatedKeys.Length; i++)
                heap.insertElement(updatedKeys[i]);
        }

        private static Element<SubnetworkPointPool> updateKey(Element<SubnetworkPointPool> element) {
            element.Key = element.Data.CumulatedWeight;
            return element;
        }

        private static Path generatePath(SubnetworkPointPool begin, SubnetworkPointPool end) {
            var path = new Path(graph.SubnetworkPointPools.Length);
            var currentSubnetworkPointPool = end;

            while (currentSubnetworkPointPool != begin) {
                if (currentSubnetworkPointPool == null)
                    return null;
                path.push(currentSubnetworkPointPool);
                currentSubnetworkPointPool = currentSubnetworkPointPool.Prev;
            }
            path.push(begin);

            return path;
        }
    }
}