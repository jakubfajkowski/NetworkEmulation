using System;

namespace NetworkUtilities.GraphAlgorithm
{
    public static class Floyd
    {

        private const double infinity = double.MaxValue;
        static private Graph graph;
        static private double[,] weights;
        static private SubnetworkPointPool[,] prev;


        static private void initialize()
        {
            weights = new double[graph.SubnetworkPointPools.Length, graph.SubnetworkPointPools.Length];
            prev = new SubnetworkPointPool[graph.SubnetworkPointPools.Length, graph.SubnetworkPointPools.Length];

            for (int i = 0; i < graph.SubnetworkPointPools.Length; i++)
            {
                for (int j = 0; j < graph.SubnetworkPointPools.Length; j++)
                {
                    weights[i, j] = infinity;
                    if (i == j) weights[i, j] = 0;

                    prev[i, j] = null;
                }
            }

            for (int i = 0; i < graph.Links.Length; i++)
            {
                int n = graph.Links[i].Begin.Id - 1;
                int m = graph.Links[i].End.Id - 1;

                weights[n, m] = graph.Links[i].Weight;
                prev[n, m] = graph.Links[i].Begin;
            }
        }

        static private void algorithmLogic()
        {
            int len = graph.SubnetworkPointPools.Length;
            for (int k = 0; k < len; k++)
            {
                //print(weights);
                //Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                //print(prev);
                //Console.WriteLine("-----------------------------------------------------------------------");

                for (int i = 0; i < len; i++)
                    for (int j = 0; j < len; j++)
                        if (weights[i, k] + weights[k, j] < weights[i, j])
                        {
                            weights[i, j] = weights[i, k] + weights[k, j];
                            prev[i, j] = prev[k, j];
                        }
            }
                
            for (int i = 0; i < len; i++)
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

        static public Path[] runAlgorithm(Graph graph_, SubnetworkPointPool begin)
        {

            graph = graph_;
            Path[] shortestPaths = new Path[graph.SubnetworkPointPools.Length];

            initialize();

            algorithmLogic();

            int len = graph.SubnetworkPointPools.Length;
            for (int i = 0; i < len; i++)
            {
                shortestPaths[i] = generatePath(begin, graph.SubnetworkPointPools[i]);
            }
            return shortestPaths;
        }

        static public Path[,] runAlgorithm(Graph graph_)
        {

            graph = graph_;
            Path[,] shortestPaths = new Path[graph.SubnetworkPointPools.Length, graph.SubnetworkPointPools.Length];

            

            initialize();

            algorithmLogic();

            int len = graph.SubnetworkPointPools.Length;
            for (int i = 0; i < len; i++)
                for (int j = 0; j < len; j++)
                {
                    shortestPaths[i,j] = generatePath(graph.SubnetworkPointPools[i], graph.SubnetworkPointPools[j]);
                }
            return shortestPaths;
        }


        static public Path[] runAlgorithm(Graph graph_, SubnetworkPointPool begin, SubnetworkPointPool end)
        {

            graph = graph_;
            Path[] shortestPaths = new Path[graph.SubnetworkPointPools.Length];

            initialize();

            algorithmLogic();

            int len = graph.SubnetworkPointPools.Length;
            
            shortestPaths[0] = generatePath(begin, end);
            return shortestPaths;
        }


        static private Path generatePath(SubnetworkPointPool begin, SubnetworkPointPool end)
        {
            Path path = new Path(graph.SubnetworkPointPools.Length);
            SubnetworkPointPool currentSubnetworkPointPool = end;
            
            while (currentSubnetworkPointPool != begin)
            {
                if (currentSubnetworkPointPool == null)
                {
                    return null;
                }
                path.push(currentSubnetworkPointPool);
                currentSubnetworkPointPool = prev[begin.Id - 1, currentSubnetworkPointPool.Id - 1];
            }
            path.push(begin);

            return path;

        }
    }
}
