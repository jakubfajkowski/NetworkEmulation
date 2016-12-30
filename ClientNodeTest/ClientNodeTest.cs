using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.element;

namespace ClientNodeTest {
    [TestClass]
    public class ClientNodeTest {
        [TestMethod]
        public void AddClientTest() {
            var parameters = new ClientNodeSerializableParameters {
                MaxAtmCellsNumberInCableCloudMessage = 100,
                ClientName = "Janusz",
                ClientTable =
                    new List<ClientTableRow>(new[]
                        {new ClientTableRow("clientName", 1, 2, 3), new ClientTableRow("clientName2", 1, 2, 3)}),
                CableCloudListeningPort = 10000,
                IpAddress = "127.0.0.1"
            };
            var expectedClientNode = new ClientNode.ClientNode(parameters);
            expectedClientNode.OnNewClientTableRow += (sender, text) => Console.WriteLine(text);

            var cn = new PrivateObject(expectedClientNode);

            cn.SetFieldOrProperty("ClientName", "Test");

            expectedClientNode.ReadClientTable(parameters);
            expectedClientNode.AddClient(new ClientTableRow("Fajka", 1, 3, 4));
        }
    }
}