using System;

namespace NetworkUtilities.GraphAlgorithm {
    public static class Floyd {
        private const double infinity = double.MaxValue;
        private static Graph graph;
        private static double[,] weights;
        private static SubnetworkPointPool[,] prev;


        private static void initialize() {
            weights = new double[graph.SubnetworkPointPools.Length, graph.SubnetworkPointPools.Length];
            prev = new SubnetworkPointPool[graph.SubnetworkPointPools.Length, graph.SubnetworkPointPools.Length];

            for (var i = 0; i < graph.SubnetworkPointPools.Length; i++)
            for (var j = 0; j < graph.SubnetworkPointPools.Length; j++) {
                weights[i, j] = infinity;
                if (i == j) weights[i, j] = 0;

                prev[i, j] = null;
            }

            for (var i = 0; i < graph.Links.Length; i++) {
                var n = graph.Links[i].Begin.Id - 1;
                var m = graph.Links[i].End.Id - 1;

                weights[n, m] = graph.Links[i].Weight;
                prev[n, m] = graph.Links[i].Begin;
            }
        }

        private static void algorithmLogic() {
            var len = graph.SubnetworkPointPools.Length;
            for (var k = 0; k < len; k++)
            for (var i = 0; i < len; i++)
            for (var j = 0; j < len; j++)
                if (weights[i, k] + weights[k, j] < weights[i, j]) {
                    weights[i, j] = weights[i, k] + weights[k, j];
                    prev[i, j] = prev[k, j];
                }

            for (var i = 0; i < len; i++)
                if (weights[i, i] < 0)
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

        public static Path[] runAlgorithm(Graph graph_, SubnetworkPointPool begin) {
            graph = graph_;
            var shortestPaths = new Path[graph.SubnetworkPointPools.Length];

            initialize();

            algorithmLogic();

            var len = graph.SubnetworkPointPools.Length;
            for (var i = 0; i < len; i++)
                shortestPaths[i] = generatePath(begin, graph.SubnetworkPointPools[i]);
            return shortestPaths;
        }

        public static Path[,] runAlgorithm(Graph graph_) {
            graph = graph_;
            var shortestPaths = new Path[graph.SubnetworkPointPools.Length, graph.SubnetworkPointPools.Length];


            initialize();

            algorithmLogic();

            var len = graph.SubnetworkPointPools.Length;
            for (var i = 0; i < len; i++)
            for (var j = 0; j < len; j++)
                shortestPaths[i, j] = generatePath(graph.SubnetworkPointPools[i], graph.SubnetworkPointPools[j]);
            return shortestPaths;
        }


        public static Path[] runAlgorithm(Graph graph_, SubnetworkPointPool begin, SubnetworkPointPool end) {
            graph = graph_;
            var shortestPaths = new Path[graph.SubnetworkPointPools.Length];

            initialize();

            algorithmLogic();

            var len = graph.SubnetworkPointPools.Length;

            shortestPaths[0] = generatePath(begin, end);
            return shortestPaths;
        }


        private static Path generatePath(SubnetworkPointPool begin, SubnetworkPointPool end) {
            var path = new Path(graph.SubnetworkPointPools.Length);
            var currentSubnetworkPointPool = end;

            while (currentSubnetworkPointPool != begin) {
                if (currentSubnetworkPointPool == null)
                    return null;
                path.push(currentSubnetworkPointPool);
                currentSubnetworkPointPool = prev[begin.Id - 1, currentSubnetworkPointPool.Id - 1];
            }
            path.push(begin);

            return path;
        }
    }
}