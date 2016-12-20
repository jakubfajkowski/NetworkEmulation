using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.network;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkNodeTest {
    [TestClass]
    public class NetworkNodeTest {
        [TestMethod]
        public void NetworkNodeSetupTest() {
            var networkNodeSerializableParameters = new NetworkNodeSerializableParameters
            {
                Id = 1,
                IpAddress = "127.0.0.1",
                CloudPort = 10000,
                NetworkManagmentSystemPort = 6666,
                NumberOfPorts = 5
            };
           
            var networkNode = new NetworkNode.NetworkNode(networkNodeSerializableParameters);
        }

       
    }
}