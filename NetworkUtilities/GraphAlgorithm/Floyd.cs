using System;

namespace NetworkUtilities.GraphAlgorithm {
    internal static class Floyd {
        private const double infinity = double.MaxValue;
        private static Graph graph;
        private static double[,] weights;
        private static Vertex[,] prev;


        private static void initialize() {
            weights = new double[graph.Vertices.Length, graph.Vertices.Length];
            prev = new Vertex[graph.Vertices.Length, graph.Vertices.Length];

            for (var i = 0; i < graph.Vertices.Length; i++)
            for (var j = 0; j < graph.Vertices.Length; j++) {
                weights[i, j] = infinity;
                if (i == j) weights[i, j] = 0;

                prev[i, j] = null;
            }

            for (var i = 0; i < graph.Edges.Length; i++) {
                var n = graph.Edges[i].Begin.Id - 1;
                var m = graph.Edges[i].End.Id - 1;

                weights[n, m] = graph.Edges[i].Weight;
                prev[n, m] = graph.Edges[i].Begin;
            }
        }

        private static void algorithmLogic() {
            var len = graph.Vertices.Length;
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

        //static private void print(Vertex[,] arr)
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

        public static Path[] runAlgorithm(Graph graph_, Vertex begin) {
            graph = graph_;
            var shortestPaths = new Path[graph.Vertices.Length];

            initialize();

            algorithmLogic();

            var len = graph.Vertices.Length;
            for (var i = 0; i < len; i++)
                shortestPaths[i] = generatePath(begin, graph.Vertices[i]);
            return shortestPaths;
        }

        public static Path[,] runAlgorithm(Graph graph_) {
            graph = graph_;
            var shortestPaths = new Path[graph.Vertices.Length, graph.Vertices.Length];


            initialize();

            algorithmLogic();

            var len = graph.Vertices.Length;
            for (var i = 0; i < len; i++)
            for (var j = 0; j < len; j++)
                shortestPaths[i, j] = generatePath(graph.Vertices[i], graph.Vertices[j]);
            return shortestPaths;
        }


        public static Path[] runAlgorithm(Graph graph_, Vertex begin, Vertex end) {
            graph = graph_;
            var shortestPaths = new Path[graph.Vertices.Length];

            initialize();

            algorithmLogic();

            var len = graph.Vertices.Length;

            shortestPaths[0] = generatePath(begin, end);
            return shortestPaths;
        }


        private static Path generatePath(Vertex begin, Vertex end) {
            var path = new Path(graph.Vertices.Length);
            var currentVertex = end;

            while (currentVertex != begin) {
                if (currentVertex == null)
                    return null;
                path.push(currentVertex);
                currentVertex = prev[begin.Id - 1, currentVertex.Id - 1];
            }
            path.push(begin);

            return path;
        }
    }
}