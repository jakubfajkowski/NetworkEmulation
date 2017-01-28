using System;
using System.Collections.Generic;

namespace NetworkUtilities.GraphAlgorithm
{
    public class Graph
    {
        public SubnetworkPointPool[] SubnetworkPointPools { get; private set; }

        private Link[] _links;
        public Link[] Links
        {
            get { return _links; }
        }

        
        private string getDataFromLine(string s, int n)
        {
            string[] stringSeparator = new string[] { " = ", " " };

            return s.Split(stringSeparator, StringSplitOptions.None)[n];
        }


        public void load(List<SubnetworkPointPool> subnetworkPointPools, List<Link> links )
        {
            SubnetworkPointPools = new SubnetworkPointPool[int.Parse(getDataFromLine(textFile[0], 1))];
            if (SubnetworkPointPools.Length == 0) throw new Exception("Zerowa liczba wierzchołków!");
            for (int i = 0; i < SubnetworkPointPools.Length; i++)
            {
                SubnetworkPointPools[i] = new SubnetworkPointPool(i+1);
            }

            _links = new Link[int.Parse(getDataFromLine(textFile[1], 1))];
            if (_links.Length == 0) throw new Exception("Zerowa liczba krawędzi!");
            for (int i = 0; i < _links.Length; i++)
            {
                int edge_id = int.Parse(getDataFromLine(textFile[2 + i], 0));
                int begin_id = int.Parse(getDataFromLine(textFile[2 + i], 1));
                int end_id = int.Parse(getDataFromLine(textFile[2 + i], 2));

                _links[i] = new Link(edge_id, SubnetworkPointPools[begin_id - 1], SubnetworkPointPools[end_id - 1]);
                _links[i].Begin.AddEdgeOut(_links[i]);
            }
        }

        //public void randomizeEdgesWeights()
        //{
        //    Random generator = new Random();
        //    for (int i = 0; i < _links.Length; i++)
        //    {
        //        double randomWeight = generator.NextDouble();
        //        _links[i].Weight = randomWeight;
        //    }
        //}
    }
}
