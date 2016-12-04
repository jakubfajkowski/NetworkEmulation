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
            XmlSerializer XML = new XmlSerializer(typeof(Node));
            TextWriter fs = new StreamWriter("testXMLserialize.xml");
            XML.Serialize(fs, node);
            Console.WriteLine(node.getPort());
            Debug.WriteLine(node.getPort());
           
            //node.serializeToXML();
           // Console.ReadLine();
        } 
    }
}
