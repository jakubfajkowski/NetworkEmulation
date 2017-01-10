using System;
using NetworkUtilities;
using NetworkUtilities.Element;
using NetworkUtilities.Serialization;

namespace NetworkNode {
    internal class Program {
        private static void Main(string[] args) {
            var joinedArgs = string.Join(" ", args);
            var parameters =
                (NetworkNodeModel)
                XmlSerializer.Deserialize(joinedArgs, typeof(NetworkNodeModel));
            var networkNode = new NetworkNode(parameters);
            Console.Title =
                $"Network Node (CC:{parameters.CableCloudDataPort}|NN:{parameters.NetworkManagmentSystemDataPort})";
        }
    }
}