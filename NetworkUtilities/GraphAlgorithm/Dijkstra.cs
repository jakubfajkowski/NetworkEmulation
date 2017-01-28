using System;

namespace NetworkUtilities.GraphAlgorithm
{
    public static class Dijkstra
    {
        private const double infinity = double.MaxValue;
        static private Graph graph;
        static public Path[] runAlgorithm(Graph graph_, SubnetworkPointPool begin)
        {
            graph = graph_;
            Path[] widestPaths = new Path[graph.SubnetworkPointPools.Length];
            PriorityQueue.Heap<SubnetworkPointPool> verticesHeap = new PriorityQueue.Heap<SubnetworkPointPool>();

            initialize(begin);
            verticesHeap.initialise(graph.SubnetworkPointPools.Length);
            for (int i = 0; i < graph.SubnetworkPointPools.Length; i++)
            {
                verticesHeap.insertElement(new PriorityQueue.Element<SubnetworkPointPool>(graph.SubnetworkPointPools[i].CumulatedWeight, graph.SubnetworkPointPools[i]));
            }

            while(verticesHeap.NumberOfElements != 0)
            {
                SubnetworkPointPool currentSubnetworkPointPool = verticesHeap.deleteMax().Data;
                if (currentSubnetworkPointPool.CumulatedWeight == 0)
                    break;

                foreach(Link e in currentSubnetworkPointPool.LinksOut)
                {
                    SubnetworkPointPool neighbor = e.End;

                    double alternate = Math.Max(neighbor.CumulatedWeight, Math.Min(currentSubnetworkPointPool.CumulatedWeight, e.Weight));

                    if (alternate > neighbor.CumulatedWeight)
                    {
                        neighbor.CumulatedWeight = alternate;
                        neighbor.Prev = currentSubnetworkPointPool;
                    }
                }
                sortHeap(ref verticesHeap);
            }

            for (int i = 0; i < graph.SubnetworkPointPools.Length; i++)
            {
                widestPaths[i] = generatePath(begin, graph.SubnetworkPointPools[i]);
            }

            return widestPaths;
        }


        static public Path[] runAlgorithm(Graph graph_, SubnetworkPointPool begin, SubnetworkPointPool end)
        {
            graph = graph_;
            Path[] widestPath = new Path[graph.SubnetworkPointPools.Length];
            PriorityQueue.Heap<SubnetworkPointPool> verticesHeap = new PriorityQueue.Heap<SubnetworkPointPool>();

            initialize(begin);
            verticesHeap.initialise(graph.SubnetworkPointPools.Length);
            for (int i = 0; i < graph.SubnetworkPointPools.Length; i++)
            {
                verticesHeap.insertElement(new PriorityQueue.Element<SubnetworkPointPool>(graph.SubnetworkPointPools[i].CumulatedWeight, graph.SubnetworkPointPools[i]));
            }

            while (verticesHeap.NumberOfElements != 0)
            {
                SubnetworkPointPool currentSubnetworkPointPool = verticesHeap.deleteMax().Data;
                if (currentSubnetworkPointPool.CumulatedWeight == 0)
                    break;
                

                foreach (Link e in currentSubnetworkPointPool.LinksOut)
                {
                    SubnetworkPointPool neighbor = e.End;


                    double alternate = Math.Max(neighbor.CumulatedWeight, Math.Min(currentSubnetworkPointPool.CumulatedWeight, e.Weight));

                    if (alternate > neighbor.CumulatedWeight)
                    {
                        neighbor.CumulatedWeight = alternate;
                        neighbor.Prev = currentSubnetworkPointPool;
                    }


                }

             
                sortHeap(ref verticesHeap);
            
            }

           
                widestPath[0] = generatePath(begin, end);
            

            return widestPath;
        }

        static public Path[,] runAlgorithm(Graph graph_)
        {
            graph = graph_;
            Path[,] widestPaths = new Path[graph.SubnetworkPointPools.Length,graph.SubnetworkPointPools.Length];
            PriorityQueue.Heap<SubnetworkPointPool> verticesHeap = new PriorityQueue.Heap<SubnetworkPointPool>();
           

            for (int b = 0; b < graph.SubnetworkPointPools.Length; b++)
            {
                SubnetworkPointPool begin = graph.SubnetworkPointPools[b];
                initialize(begin);
                verticesHeap.initialise(graph.SubnetworkPointPools.Length);
                for (int i = 0; i < graph.SubnetworkPointPools.Length; i++)
                {
                    verticesHeap.insertElement(new PriorityQueue.Element<SubnetworkPointPool>(graph.SubnetworkPointPools[i].CumulatedWeight, graph.SubnetworkPointPools[i]));
                }

                while (verticesHeap.NumberOfElements != 0)
                {
                    SubnetworkPointPool currentSubnetworkPointPool = verticesHeap.deleteMax().Data;
                    if (currentSubnetworkPointPool.CumulatedWeight == 0)
                        break;


                    foreach (Link e in currentSubnetworkPointPool.LinksOut)
                    {
                        SubnetworkPointPool neighbor = e.End;


                        double alternate = Math.Max(neighbor.CumulatedWeight, Math.Min(currentSubnetworkPointPool.CumulatedWeight, e.Weight));

                        if (alternate > neighbor.CumulatedWeight)
                        {
                            neighbor.CumulatedWeight = alternate;
                            neighbor.Prev = currentSubnetworkPointPool;
                        }


                    }


                    sortHeap(ref verticesHeap);

                }

                for (int i = 0; i < graph.SubnetworkPointPools.Length; i++)
                {
                    widestPaths[b,i] = generatePath(begin, graph.SubnetworkPointPools[i]);
                }



            }
            return widestPaths;
            
        }
        

        static private void initialize(SubnetworkPointPool begin)
        {

            for (int i = 0; i < graph.SubnetworkPointPools.Length; i++)
            {
                graph.SubnetworkPointPools[i].CumulatedWeight = 0;
                graph.SubnetworkPointPools[i].Prev = null;
            }
                
            graph.SubnetworkPointPools[begin.Id - 1].CumulatedWeight = infinity;

            begin.Prev = begin;
        }

        static private void sortHeap(ref PriorityQueue.Heap<SubnetworkPointPool> heap)
        {
            PriorityQueue.Element<SubnetworkPointPool>[] updatedKeys = new PriorityQueue.Element<SubnetworkPointPool>[heap.NumberOfElements];
            for (int i = 0; i < updatedKeys.Length; i++)
            {
                updatedKeys[i] = updateKey(heap.deleteMax());
            }
                for (int i = 0; i < updatedKeys.Length; i++)
            {
                heap.insertElement(updatedKeys[i]);
            }
        }

        static private PriorityQueue.Element<SubnetworkPointPool> updateKey(PriorityQueue.Element<SubnetworkPointPool> element)
        {
            element.Key = element.Data.CumulatedWeight;
            return element;
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
                currentSubnetworkPointPool = currentSubnetworkPointPool.Prev;
            }
            path.push(begin);

            return path;
        }
    }
}
