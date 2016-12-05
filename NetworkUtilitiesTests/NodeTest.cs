using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace NetworkUtilitiesTests
{
    [TestClass]
    public class NodeTest
    {
        [TestMethod]
        public void testNodeConstructor()
        {
            
            Node node = new Node(5431, 3451);
            Console.Write(node.toXML());
        } 
    }
}
