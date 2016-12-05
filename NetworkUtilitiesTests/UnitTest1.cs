using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace NetworkUtilitiesTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            
            Node node = new Node(5431);
            Console.Write(node.ToXML());
        } 
    }
}
