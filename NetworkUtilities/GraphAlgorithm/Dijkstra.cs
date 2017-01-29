using System;
using NetworkUtilities.GraphAlgorithm.PriorityQueue;

namespace NetworkUtilities.GraphAlgorithm {
    public static class Dijkstra {
        private const double Infinity = double.MaxValue;
        private static Graph _graph;

        public static Path[] RunAlgorithm(Graph graph_, SubnetworkPointPool begin) {
            _graph = graph_;
            var widestPaths = new Path[_graph.SubnetworkPointPools.Length];
            var verticesHeap = new Heap<SubnetworkPointPool>();

            Initialize(begin);
            verticesHeap.Initialise(_graph.SubnetworkPointPools.Length);
            for (var i = 0; i < _graph.SubnetworkPointPools.Length; i++)
                verticesHeap.InsertElement(
                    new Element<SubnetworkPointPool>(_graph.SubnetworkPointPools[i].CumulatedWeight,
                        _graph.SubnetworkPointPools[i]));

            while (verticesHeap.NumberOfElements != 0) {
                var currentSubnetworkPointPool = verticesHeap.DeleteMax().Data;
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
                SortHeap(ref verticesHeap);
            }

            for (var i = 0; i < _graph.SubnetworkPointPools.Length; i++)
                widestPaths[i] = GeneratePath(begin, _graph.SubnetworkPointPools[i]);

            return widestPaths;
        }


        public static Path[] RunAlgorithm(Graph graph_, SubnetworkPointPool begin, SubnetworkPointPool end) {
            _graph = graph_;
            var widestPath = new Path[_graph.SubnetworkPointPools.Length];
            var verticesHeap = new Heap<SubnetworkPointPool>();

            Initialize(begin);
            verticesHeap.Initialise(_graph.SubnetworkPointPools.Length);
            for (var i = 0; i < _graph.SubnetworkPointPools.Length; i++)
                verticesHeap.InsertElement(
                    new Element<SubnetworkPointPool>(_graph.SubnetworkPointPools[i].CumulatedWeight,
                        _graph.SubnetworkPointPools[i]));

            while (verticesHeap.NumberOfElements != 0) {
                var currentSubnetworkPointPool = verticesHeap.DeleteMax().Data;
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


                SortHeap(ref verticesHeap);
            }


            widestPath[0] = GeneratePath(begin, end);


            return widestPath;
        }

        public static Path[,] RunAlgorithm(Graph graph_) {
            _graph = graph_;
            var widestPaths = new Path[_graph.SubnetworkPointPools.Length, _graph.SubnetworkPointPools.Length];
            var verticesHeap = new Heap<SubnetworkPointPool>();


            for (var b = 0; b < _graph.SubnetworkPointPools.Length; b++) {
                var begin = _graph.SubnetworkPointPools[b];
                Initialize(begin);
                verticesHeap.Initialise(_graph.SubnetworkPointPools.Length);
                for (var i = 0; i < _graph.SubnetworkPointPools.Length; i++)
                    verticesHeap.InsertElement(
                        new Element<SubnetworkPointPool>(_graph.SubnetworkPointPools[i].CumulatedWeight,
                            _graph.SubnetworkPointPools[i]));

                while (verticesHeap.NumberOfElements != 0) {
                    var currentSubnetworkPointPool = verticesHeap.DeleteMax().Data;
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


                    SortHeap(ref verticesHeap);
                }

                for (var i = 0; i < _graph.SubnetworkPointPools.Length; i++)
                    widestPaths[b, i] = GeneratePath(begin, _graph.SubnetworkPointPools[i]);
            }
            return widestPaths;
        }


        private static void Initialize(SubnetworkPointPool begin) {
            for (var i = 0; i < _graph.SubnetworkPointPools.Length; i++) {
                _graph.SubnetworkPointPools[i].CumulatedWeight = 0;
                _graph.SubnetworkPointPools[i].Prev = null;
            }

            _graph.SubnetworkPointPools[begin.Id - 1].CumulatedWeight = Infinity;

            begin.Prev = begin;
        }

        private static void SortHeap(ref Heap<SubnetworkPointPool> heap) {
            var updatedKeys = new Element<SubnetworkPointPool>[heap.NumberOfElements];
            for (var i = 0; i < updatedKeys.Length; i++)
                updatedKeys[i] = UpdateKey(heap.DeleteMax());
            for (var i = 0; i < updatedKeys.Length; i++)
                heap.InsertElement(updatedKeys[i]);
        }

        private static Element<SubnetworkPointPool> UpdateKey(Element<SubnetworkPointPool> element) {
            element.Key = element.Data.CumulatedWeight;
            return element;
        }

        private static Path GeneratePath(SubnetworkPointPool begin, SubnetworkPointPool end) {
            var path = new Path(_graph.SubnetworkPointPools.Length);
            var currentSubnetworkPointPool = end;

            while (currentSubnetworkPointPool != begin) {
                if (currentSubnetworkPointPool == null)
                    return null;
                path.Push(currentSubnetworkPointPool);
                currentSubnetworkPointPool = currentSubnetworkPointPool.Prev;
            }
            path.Push(begin);

            return path;
        }
    }
}