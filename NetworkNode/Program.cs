using System;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkNode {
    internal class Program {
        private static void Main(string[] args) {
            string joinedArgs = string.Join(" ", args);          
            var parameters = (NetworkNodeSerializableParameters)XmlSerializer.Deserialize(joinedArgs, typeof(NetworkNodeSerializableParameters));           
            var networkNode = new NetworkNode(parameters);
            Console.Title = $"Network Node ({parameters.CableCloudDataPort})";
        }
    }
}