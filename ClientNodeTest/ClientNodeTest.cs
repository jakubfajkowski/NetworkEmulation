using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using ClientNode;
using System.Collections.Generic;

namespace ClientNodeTest
{
    [TestClass]
    public class ClientNodeTest
    {
        public List<ATMCell> atmCellList = new List<ATMCell>();
      
        [TestMethod]
        public void createATMCellTest() {
            var client = new ClientNode.ClientNode();
            String text = "Test text to check if createATMCellTest method works just right.";
            atmCellList = client.createATMCell(1, 1, text);

            Assert.Equals(2, atmCellList.Count);
        }
        //public void addClientTest() {
        //    var clientNode = new MainForm();
        //}
    }
}
