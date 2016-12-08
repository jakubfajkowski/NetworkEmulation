using System;
using ClientNode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using System.Collections.Generic;

namespace ClientNodeTest
{
    [TestClass]
    public class ClientNodeTest
    {
        public List<ATMCell> atmCellList = new List<ATMCell>();
        [TestMethod]
        public void createATMCellTest() {
            var clientNode = new ClientNode.ClientNode("Jarek");
            String text = "Test text to check if createATMCellTest method works just right.";
            clientNode.createATMCell(1, 1, text, atmCellList);

            Assert.Equals(2, atmCellList.Count);
        }
        //public void addClientTest() {
        //    var clientNode = new MainForm();
        //}
    }
}
