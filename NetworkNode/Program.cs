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
                  
            /*
            var networkNodeSerializableParameters = new NetworkNodeModel
            {
                NumberOfPorts = 8,
                CableCloudListeningPort = 10000,
                IpAddress = "127.0.0.1",
                CableCloudDataPort = PortRandomizer.RandomFreePort(),
                NetworkManagmentSystemListeningPort = 6666,
                NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort()
            };
            var networkNode = new NetworkNode(networkNodeSerializableParameters);

           // for (int i=0;i<5;i++)
           //  networkNode.linkResourceManager.getNewLabels(1);

            Console.ReadLine();
            */
        }
    }
}