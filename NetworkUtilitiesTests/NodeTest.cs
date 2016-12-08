using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using System.Net.Sockets;


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

        [TestMethod]
        public void testNodeRecieve()
        {
            try {
            Node node = new Node(5431, 3451);
            string server = "127.0.0.1";
            string message = "You can do it";
            Int32 port = 3451;
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, port);
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);
            Debug.WriteLine("Sent: {0}", message);
            // Receive the TcpServer.response.
            // Buffer to store the response bytes.
            data = new Byte[512];
            // String to store the response ASCII representation.
            String responseData = String.Empty;
            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
            Debug.WriteLine("Received: {0}", responseData);
            Debug.WriteLine("Received: {0}", responseData);
             Assert.AreEqual(message,responseData);
            // Close everything.
            stream.Close();
            client.Close();
               } 
              catch (ArgumentNullException e) 
              {
                Console.WriteLine("ArgumentNullException: {0}", e);
                Debug.WriteLine("ArgumentNullException: {0}", e);
              } 
              catch (SocketException e) 
              {
                Console.WriteLine("SocketException: {0}", e);
                Debug.WriteLine("SocketException: {0}", e);
              }

            Console.WriteLine("\n Press Enter to continue...");
              Debug.WriteLine("\n Press Enter to continue...");
              
         }
    }
}
