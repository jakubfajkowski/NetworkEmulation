using System;
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
            networkNode.UpdateState += (sender, state) => Console.WriteLine(state);
            networkNode.Initialize();
            Console.Title =
                $"Network Node ({parameters.NetworkAddress})";
        }
    }
}