using System;

namespace NetworkUtilities.GraphAlgorithm {
    public static class Floyd {
        private const double Infinity = double.MaxValue;
        private static Graph _graph;
        private static double[,] _weights;
        private static SubnetworkPointPool[,] _prev;


        private static void Initialize() {
            _weights = new double[_graph.SubnetworkPointPools.Length, _graph.SubnetworkPointPools.Length];
            _prev = new SubnetworkPointPool[_graph.SubnetworkPointPools.Length, _graph.SubnetworkPointPools.Length];

            for (var i = 0; i < _graph.SubnetworkPointPools.Length; i++)
            for (var j = 0; j < _graph.SubnetworkPointPools.Length; j++) {
                _weights[i, j] = Infinity;
                if (i == j) _weights[i, j] = 0;

                _prev[i, j] = null;
            }

            for (var i = 0; i < _graph.Links.Length; i++) {
                var n = _graph.Links[i].Begin.Id - 1;
                var m = _graph.Links[i].End.Id - 1;

                _weights[n, m] = _graph.Links[i].Weight;
                _prev[n, m] = _graph.Links[i].Begin;
            }
        }

        private static void AlgorithmLogic() {
            var len = _graph.SubnetworkPointPools.Length;
            for (var k = 0; k < len; k++)
            for (var i = 0; i < len; i++)
            for (var j = 0; j < len; j++)
                if (_weights[i, k] + _weights[k, j] < _weights[i, j]) {
                    _weights[i, j] = _weights[i, k] + _weights[k, j];
                    _prev[i, j] = _prev[k, j];
                }

            for (var i = 0; i < len; i++)
                if (_weights[i, i] < 0)
                    throw new ArgumentException("Cykle ujemne");
            //print(weights);
            //Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            //print(prev);
            //Console.WriteLine("-----------------------------------------------------------------------");
        }

        //static private void print(double[,] arr)
        //{
        //    var rowCount = arr.GetLength(0);
        //    var colCount = arr.GetLength(1);
        //    for (int row = 0; row < rowCount; row++)
        //    {
        //        for (int col = 0; col < colCount; col++)
        //            Console.Write(String.Format("{0}\t", arr[row, col]));
        //        Console.WriteLine();
        //    }
        //}

        //static private void print(SubnetworkPointPool[,] arr)
        //{
        //    var rowCount = arr.GetLength(0);
        //    var colCount = arr.GetLength(1);
        //    for (int row = 0; row < rowCount; row++)
        //    {
        //        for (int col = 0; col < colCount; col++)
        //            if(arr[row,col] != null)
        //            {
        //                Console.Write(String.Format("{0}\t", arr[row, col].Id));
        //            }else
        //            {
        //                Console.Write(String.Format("{0}\t", "-"));
        //            }
        //        Console.WriteLine();
        //    }
        //}

        public static Path[] RunAlgorithm(Graph graph_, SubnetworkPointPool begin) {
            _graph = graph_;
            var shortestPaths = new Path[_graph.SubnetworkPointPools.Length];

            Initialize();

            AlgorithmLogic();

            var len = _graph.SubnetworkPointPools.Length;
            for (var i = 0; i < len; i++)
                shortestPaths[i] = GeneratePath(begin, _graph.SubnetworkPointPools[i]);
            return shortestPaths;
        }

        public static Path[,] RunAlgorithm(Graph graph_) {
            _graph = graph_;
            var shortestPaths = new Path[_graph.SubnetworkPointPools.Length, _graph.SubnetworkPointPools.Length];


            Initialize();

            AlgorithmLogic();

            var len = _graph.SubnetworkPointPools.Length;
            for (var i = 0; i < len; i++)
            for (var j = 0; j < len; j++)
                shortestPaths[i, j] = GeneratePath(_graph.SubnetworkPointPools[i], _graph.SubnetworkPointPools[j]);
            return shortestPaths;
        }


        public static Path[] RunAlgorithm(Graph graph_, SubnetworkPointPool begin, SubnetworkPointPool end) {
            _graph = graph_;
            var shortestPaths = new Path[_graph.SubnetworkPointPools.Length];

            Initialize();

            AlgorithmLogic();

            var len = _graph.SubnetworkPointPools.Length;

            shortestPaths[0] = GeneratePath(begin, end);
            return shortestPaths;
        }


        private static Path GeneratePath(SubnetworkPointPool begin, SubnetworkPointPool end) {
            var path = new Path(_graph.SubnetworkPointPools.Length);
            var currentSubnetworkPointPool = end;

            while (currentSubnetworkPointPool != begin) {
                if (currentSubnetworkPointPool == null)
                    return null;
                path.Push(currentSubnetworkPointPool);
                currentSubnetworkPointPool = _prev[begin.Id - 1, currentSubnetworkPointPool.Id - 1];
            }
            path.Push(begin);

            return path;
        }
    }
}