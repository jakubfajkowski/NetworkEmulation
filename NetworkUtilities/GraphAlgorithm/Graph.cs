using System;
using System.Collections.Generic;

namespace NetworkUtilities.GraphAlgorithm {
    public class Graph {
        public SubnetworkPointPool[] SubnetworkPointPools { get; private set; }

        public Link[] Links { get; set; }


        private string getDataFromLine(string s, int n) {
            string[] stringSeparator = {" = ", " "};

            return s.Split(stringSeparator, StringSplitOptions.None)[n];
        }


        public void Load(Link[] links) {
            Links = links;
            var subnetworkPointPools = new List<SubnetworkPointPool>();
            foreach (var link in Links) {
                if (!subnetworkPointPools.Contains(link.Begin))
                    subnetworkPointPools.Add(link.Begin);
                if (!subnetworkPointPools.Contains(link.End))
                    subnetworkPointPools.Add(link.End);
            }
            SubnetworkPointPools = subnetworkPointPools.ToArray();
        }

        public void LoadDeprecated(List<string> textFile) {
            SubnetworkPointPools = new SubnetworkPointPool[int.Parse(getDataFromLine(textFile[0], 1))];
            if (SubnetworkPointPools.Length == 0) throw new Exception("Zerowa liczba wierzchołków!");
            for (var i = 0; i < SubnetworkPointPools.Length; i++)
                SubnetworkPointPools[i] = new SubnetworkPointPool(i + 1);

            Links = new Link[int.Parse(getDataFromLine(textFile[1], 1))];
            if (Links.Length == 0) throw new Exception("Zerowa liczba krawędzi!");
            for (var i = 0; i < Links.Length; i++) {
                var edge_id = int.Parse(getDataFromLine(textFile[2 + i], 0));
                var begin_id = int.Parse(getDataFromLine(textFile[2 + i], 1));
                var end_id = int.Parse(getDataFromLine(textFile[2 + i], 2));

                Links[i] = new Link(edge_id, SubnetworkPointPools[begin_id - 1], SubnetworkPointPools[end_id - 1]);
                Links[i].Begin.AddEdgeOut(Links[i]);
            }
        }

        //    {
        //    for (int i = 0; i < _links.Length; i++)
        //    Random generator = new Random();
        //{
        //public void randomizeEdgesWeights()
        //        double randomWeight = generator.NextDouble();
        //        _links[i].Weight = randomWeight;
        //    }
        //}
    }
}