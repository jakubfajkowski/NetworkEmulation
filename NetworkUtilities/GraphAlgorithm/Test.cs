using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NetworkUtilities.GraphAlgorithm {
    internal static class Test {
        private static Graph graph;
        private static long averageTimeDijkstra;
        private static long averageTimeFloyd;
        private static long testTime;

        private static void initialize(string path) {
            graph = new Graph();
            using (var streamReader = new StreamReader(path)) {
                var textFile = new List<string>();

                while (streamReader.EndOfStream == false) {
                    var line = streamReader.ReadLine();

                    if (!line.Contains("#") && line != "")
                        textFile.Add(line);
                }
                graph.load(textFile);
            }
        }

        private static void findShortestPaths(int numberOfTests) {
            var testTimeStopwatch = new Stopwatch();
            var algorithmStopwatch = new Stopwatch();

            testTimeStopwatch.Start();
            for (var i = 0; i < numberOfTests; i++) {
                //Console.WriteLine("Algorytm Dijkstry:");
                graph.randomizeEdgesWeights();
                algorithmStopwatch.Restart();
                Dijkstra.runAlgorithm(graph); //printPaths(Dijkstra.runAlgorithm(graph));
                algorithmStopwatch.Stop();
                averageTimeDijkstra += algorithmStopwatch.ElapsedTicks;


                //Console.WriteLine("Algorytm Dijkstry od:");
                //graph.randomizeEdgesWeights();
                //algorithmStopwatch.Restart();
                //printPaths(Dijkstra.runAlgorithm(graph, graph.Vertices[0]));
                //algorithmStopwatch.Stop();
                //averageTimeDijkstra += algorithmStopwatch.ElapsedTicks;

                //Console.WriteLine("Algorytm Dijkstry od do:");
                //graph.randomizeEdgesWeights();
                //algorithmStopwatch.Restart();
                //printPaths(Dijkstra.runAlgorithm(graph, graph.Vertices[0], graph.Vertices[1]));
                //algorithmStopwatch.Stop();
                //averageTimeDijkstra += algorithmStopwatch.ElapsedTicks;


                //Console.WriteLine("Algorytm Floyda:");
                graph.randomizeEdgesWeights();
                algorithmStopwatch.Restart();
                Floyd.runAlgorithm(graph); //printPaths(Floyd.runAlgorithm(graph));
                algorithmStopwatch.Stop();
                averageTimeFloyd += algorithmStopwatch.ElapsedTicks;

                //Console.WriteLine("Algorytm Floyda od:");
                //graph.randomizeEdgesWeights();
                //algorithmStopwatch.Restart();
                //printPaths(Floyd.runAlgorithm(graph, graph.Vertices[4]));
                //algorithmStopwatch.Stop();
                //averageTimeFloyd += algorithmStopwatch.ElapsedTicks;

                //Console.WriteLine("Algorytm Floyda od do:");
                //graph.randomizeEdgesWeights();
                //algorithmStopwatch.Restart();
                //printPaths(Floyd.runAlgorithm(graph, graph.Vertices[4], graph.Vertices[3]));
                //algorithmStopwatch.Stop();
                //averageTimeFloyd += algorithmStopwatch.ElapsedTicks;
            }
            averageTimeDijkstra /= numberOfTests;
            averageTimeFloyd /= numberOfTests;
            testTimeStopwatch.Stop();
            testTime = testTimeStopwatch.ElapsedTicks;
        }

        private static void printPaths(Path[] paths) {
            foreach (var p in paths)
                if (p != null) {
                    foreach (var v in p.Vertices)
                        Console.Write(v.Id + " ");
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

        private static void printPaths(Path[,] paths) {
            foreach (var p in paths)
                if (p != null) {
                    foreach (var v in p.Vertices)
                        Console.Write(v.Id + " ");
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

        private static void printResults() {
            Console.WriteLine("Średni czas dla algorytmu Dijkstry: " + new TimeSpan(averageTimeDijkstra));
            Console.WriteLine("Średni czas dla algorytmu Floyda: " + new TimeSpan(averageTimeFloyd));
            Console.WriteLine("Całkowity czas trwania testu: " + new TimeSpan(testTime));
        }

        public static void run(string path, int numberOfTests) {
            initialize(path);
            findShortestPaths(numberOfTests);
            printResults();
        }

        public static void generateGraph(int n) {
            using (var file =
                new StreamWriter("graf_input.txt")) {
                file.WriteLine("WEZLY = {0}", n);
                file.WriteLine("LACZA = {0}", n * (n - 1));
                var id = 1;
                for (var i = 0; i < n; i++)
                for (var j = 0; j < n; j++)
                    if (i != j) file.WriteLine("{0} {1} {2}", id++, i + 1, j + 1);
            }
        }
    }
}