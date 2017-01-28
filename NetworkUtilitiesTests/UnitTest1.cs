using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.GraphAlgorithm;
using Path = NetworkUtilities.GraphAlgorithm.Path;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class UnitTest1 {
        static private Graph graph;
        static private long averageTimeDijkstra;
        static private long averageTimeFloyd;
        static private long testTime;


        [TestMethod]
        public void TestMethod1() {
            generateGraph(500);


            run("D:\\Projects\\_Visual Studio\\NetworkEmulation\\NetworkUtilitiesTests\\bin\\Release\\graf_input.txt", 1);
        }

        [TestMethod]
        public void TestRouteTableQuery () {
            var snpps = new List<SubnetworkPointPool> {
                new SubnetworkPointPool() {NetworkSnppAddress = new NetworkAddress("1.2.1"), Id = 1},
                new SubnetworkPointPool() {NetworkSnppAddress = new NetworkAddress("1.2.2"), Id = 2},
                new SubnetworkPointPool() {NetworkSnppAddress = new NetworkAddress("1.2.3"), Id = 3},
                new SubnetworkPointPool() {NetworkSnppAddress = new NetworkAddress("1.2.4"), Id = 4},
                new SubnetworkPointPool() {NetworkSnppAddress = new NetworkAddress("1.2.5"), Id = 5}
            };

            var links = new Link[8];
            links[0]= new Link(5, snpps[0],snpps[1]);
            links[1]= new Link(4, snpps[0],snpps[2]);
            links[2]= new Link(20, snpps[2],snpps[1]);
            links[3]= new Link(10, snpps[0],snpps[3]);
            links[4]= new Link(15, snpps[4],snpps[1]);
            links[5]= new Link(15, snpps[3],snpps[4]);
            links[6]= new Link(20, snpps[3],snpps[2]);
            links[7]= new Link(20, snpps[2],snpps[4]);

            graph= new Graph();
            graph.Load(links);

            var rcAddress = new NetworkAddress("1");
            var routingController = new RoutingController(rcAddress);
            routingController._linkList = links.ToList();

            var sm = new SignallingMessage {
                DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.RoutingController,
                Operation = SignallingMessageOperation.RouteTableQuery,
                DestinationAddress = rcAddress,
                Payload = new object[] {new[] {snpps[0], snpps[2]}, 4},
                SourceAddress = new NetworkAddress("2")
                };

            routingController.ReceiveMessage(sm);
        

        }

        

        static private void initialize(string path) {
            graph = new Graph();
            using (StreamReader streamReader = new StreamReader(path)) {
                List<string> textFile = new List<string>();

                while (streamReader.EndOfStream == false) {
                    string line = streamReader.ReadLine();

                    if (!line.Contains("#") && line != "") {
                        textFile.Add(line);
                    }
                }
                graph.LoadDeprecated(textFile);
            }
        }

        static private void findShortestPaths(int numberOfTests) {
            Stopwatch testTimeStopwatch = new Stopwatch();
            Stopwatch algorithmStopwatch = new Stopwatch();

            testTimeStopwatch.Start();
            for (int i = 0; i < numberOfTests; i++) {
                Console.WriteLine("Algorytm Dijkstry:");
                algorithmStopwatch.Restart();
                Dijkstra.runAlgorithm(graph); //printPaths(Dijkstra.runAlgorithm(graph));
                algorithmStopwatch.Stop();
                averageTimeDijkstra += algorithmStopwatch.ElapsedTicks;


                //Console.WriteLine("Algorytm Dijkstry od:");
                //graph.randomizeEdgesWeights();
                //algorithmStopwatch.Restart();
                //printPaths(Dijkstra.runAlgorithm(graph, graph.SubnetworkPointPools[0]));
                //algorithmStopwatch.Stop();
                //averageTimeDijkstra += algorithmStopwatch.ElapsedTicks;

                //Console.WriteLine("Algorytm Dijkstry od do:");
                //graph.randomizeEdgesWeights();
                //algorithmStopwatch.Restart();
                //printPaths(Dijkstra.runAlgorithm(graph, graph.SubnetworkPointPools[0], graph.SubnetworkPointPools[1]));
                //algorithmStopwatch.Stop();
                //averageTimeDijkstra += algorithmStopwatch.ElapsedTicks;


                //Console.WriteLine("Algorytm Floyda:");
                algorithmStopwatch.Restart();
                Floyd.runAlgorithm(graph); //printPaths(Floyd.runAlgorithm(graph));
                algorithmStopwatch.Stop();
                averageTimeFloyd += algorithmStopwatch.ElapsedTicks;

                //Console.WriteLine("Algorytm Floyda od:");
                //graph.randomizeEdgesWeights();
                //algorithmStopwatch.Restart();
                //printPaths(Floyd.runAlgorithm(graph, graph.SubnetworkPointPools[4]));
                //algorithmStopwatch.Stop();
                //averageTimeFloyd += algorithmStopwatch.ElapsedTicks;

                //Console.WriteLine("Algorytm Floyda od do:");
                //graph.randomizeEdgesWeights();
                //algorithmStopwatch.Restart();
                printPaths(Floyd.runAlgorithm(graph, graph.SubnetworkPointPools[4], graph.SubnetworkPointPools[3]));
                //algorithmStopwatch.Stop();
                //averageTimeFloyd += algorithmStopwatch.ElapsedTicks;
            }
            averageTimeDijkstra /= numberOfTests;
            averageTimeFloyd /= numberOfTests;
            testTimeStopwatch.Stop();
            testTime = testTimeStopwatch.ElapsedTicks;
        }

        static private void printPaths(Path[] paths) {
            foreach (Path p in paths) {
                if (p != null) {
                    foreach (SubnetworkPointPool v in p.SubnetworkPointPools) {
                        Console.Write(v.Id + " ");
                    }
                    Console.WriteLine();
                    //if (p.MinWeight == double.MaxValue)
                    //{
                    //    Console.WriteLine(" min: infinity" + " sum: " + p.SumWeight);
                    //}
                    //else
                    //{
                    //    Console.WriteLine(" min: " + p.MinWeight + " sum: " + p.SumWeight);
                    //}
                }
            }
        }

        static private void printPaths(Path[,] paths) {
            foreach (Path p in paths) {
                if (p != null) {
                    foreach (SubnetworkPointPool v in p.SubnetworkPointPools) {
                        Console.Write(v.Id + " ");
                    }
                    Console.WriteLine();
                    //if (p.MinWeight == double.MaxValue)
                    //{
                    //    Console.WriteLine(" min: infinity" + " sum: " + p.SumWeight);
                    //}
                    //else
                    //{
                    //    Console.WriteLine(" min: " + p.MinWeight + " sum: " + p.SumWeight);
                    //}
                }
            }
        }

        static private void printResults() {
            Console.WriteLine("Średni czas dla algorytmu Dijkstry: " + new TimeSpan(averageTimeDijkstra));
            Console.WriteLine("Średni czas dla algorytmu Floyda: " + new TimeSpan(averageTimeFloyd));
            Console.WriteLine("Całkowity czas trwania testu: " + new TimeSpan(testTime));
        }

        static public void run(string path, int numberOfTests) {
            initialize(path);
            findShortestPaths(numberOfTests);
            printResults();
        }

        static public void generateGraph(int n) {
            using (StreamWriter file =
                new StreamWriter("graf_input.txt")) {
                file.WriteLine("WEZLY = {0}", n);
                file.WriteLine("LACZA = {0}", n*(n - 1));
                int id = 1;
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (i != j) file.WriteLine("{0} {1} {2}", id++, i + 1, j + 1);
            }
            //string graphStr="";
            //graphStr += "WEZLY = "+ n;
            //graphStr += "\nLACZA = "+ n * (n -1);
            //int id = 1;
            //for (int i = 0; i < n; i++)
            //    for (int j = 0; j < n; j++)
            //        if (i != j) graphStr+= (id++) +" "+ (i + 1) +" " +(j + 1);
            //return graphStr;
        }
    }
}