using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.Element;

namespace ClientNodeTest {
    [TestClass]
    public class ClientNodeTest {
        [TestMethod]
        public void AddClientTest() {
            var parameters = new ClientNodeModel {
                MaxAtmCellsNumberInCableCloudMessage = 100,
                ClientName = "Janusz",
                CableCloudListeningPort = 10000,
                IpAddress = "127.0.0.1"
            };
        }
    }
}