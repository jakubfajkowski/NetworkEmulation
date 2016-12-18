using System;

namespace NetworkNode {
    internal class Program {
        private static void Main(string[] args) {
            var networkNode = new NetworkNode(1,1);
            Console.WriteLine("Długosc: " +args.Length);
            Console.ReadLine();
            networkNode.shutdown();
            networkNode.startThread();
        }
    }
}