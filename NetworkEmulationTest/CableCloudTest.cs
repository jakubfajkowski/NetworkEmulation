using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation;
using NetworkUtilities;

namespace NetworkEmulationTest {
    [TestClass]
    public class CableCloudTest {
        public CableCloudTest() {
            this._random = new Random();
            _cableCloudIpEndpoint = new IPEndPoint(IPAddress.Loopback, 10000);
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        private Random _random;
        private readonly IPEndPoint _cableCloudIpEndpoint;
        private byte[] _bytesToSend;
        private byte[] _bytesRecieved;

        [TestMethod]
        public void CableCloudBindEndpointTest() {
            CableCloud cableCloud = new CableCloud();
            int port = 10001;

            var listenerTask = StartTcpListener(port, Listen);

            ConnectToCableCloud(port);
            listenerTask.Wait();

            var nodesTcpClients =
                (Dictionary<int, TcpClient>)new PrivateObject(cableCloud).GetField("_nodesTcpClients");
            Assert.AreEqual(1, nodesTcpClients.Count);
        }

        [TestMethod]
        public void CableCloudConnectNodeTest() {
            CableCloud cableCloud = new CableCloud();
            while (!cableCloud.Online) ;
            Node node = new Node();
            while (!node.isOnline()) ;

            var nodesTcpClients = (Dictionary<int, TcpClient>)new PrivateObject(cableCloud).GetField("_nodesTcpClients");
            Assert.AreEqual(1, nodesTcpClients.Count);
        }

        [TestMethod]
        public void CableCloudPassMessageTest() {
            CableCloud cableCloud = new CableCloud();
            cableCloud.OnUpdateState += (sender, state) => Console.WriteLine(state);
            int port1 = 10001;
            int port2 = 10002;
            int port3 = 10003;

            var output = new SocketNodePortPair(1, port1);
            var input1 = new SocketNodePortPair(1, port2);
            var input2 = new SocketNodePortPair(1, port3);

            cableCloud.AddLink(input1, output);
            cableCloud.AddLink(input2, output);
            _bytesToSend = CableCloudMessage.serialize(CreateCableCloudMessage(1, 100));

            var listenerTask1 = StartTcpListener(port1, RecieveMessage);
            ConnectToCableCloud(port1);
            var listenerTask2 = StartTcpListener(port2, SendMessage);
            ConnectToCableCloud(port2);
            var listenerTask3 = StartTcpListener(port3, SendMessage);
            ConnectToCableCloud(port3);

            Task.WaitAll(listenerTask1, listenerTask2, listenerTask3);

            for (int i = 0; i < _bytesToSend.Length; i++) {
                Assert.AreEqual(_bytesToSend[i], _bytesRecieved[i]);
            }
        }

        [TestMethod]
        public void CableCloudXmlSerializationTest() {
            CableCloud cableCloud = new CableCloud();
            var cc = new PrivateObject(cableCloud);

            for (int i = 0; i < 10; i++) {
                cableCloud.AddLink(RandomSocketNodePortPair(), RandomSocketNodePortPair());
            }

            var expected = ((SerializableDictionary<SocketNodePortPair, SocketNodePortPair>) cc.GetField("_linkDictionary")).Count;

            var serializedCloud = Serialize(cableCloud);

            cableCloud.AddLink(RandomSocketNodePortPair(), RandomSocketNodePortPair());

            TextReader textReader = new StringReader(serializedCloud);
            var settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            using (XmlReader xmlReader = XmlReader.Create(textReader, settings)) {
                cableCloud.ReadXml(xmlReader);
            }

            var actual = ((SerializableDictionary<SocketNodePortPair, SocketNodePortPair>) cc.GetField("_linkDictionary")).Count;

            Assert.AreEqual(expected, actual);
        }

        public string Serialize(object obj) {
            XmlSerializer xsSubmit = new XmlSerializer(obj.GetType());
            var subReq = obj;
            var xml = "";
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            using (var sww = new StringWriter()) {
                using (XmlWriter writer = XmlWriter.Create(sww, settings)) {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString(); // Your XML
                }
            }

            return xml;
        }

        private static CableCloudMessage CreateCableCloudMessage(int linkNumber, int atmCellsNumber) {
            var cableCloudMessage = new CableCloudMessage(linkNumber);
            for (int i = 0; i < atmCellsNumber; i++) {
                cableCloudMessage.add(new ATMCell(1, 1, new byte[48]));
            }

            return cableCloudMessage;
        }

        private SocketNodePortPair RandomSocketNodePortPair() {
            return new SocketNodePortPair(_random.Next(), _random.Next());
        }

        private void ConnectToCableCloud(int port) {
            UdpClient udpClient = new UdpClient();

            byte[] bytesToSend = BitConverter.GetBytes(port);
            udpClient.Send(bytesToSend, bytesToSend.Length, _cableCloudIpEndpoint);
        }

        private Task StartTcpListener(int port, Func<TcpListener, Task> function) {
            var nodeIpEndpoint = new IPEndPoint(IPAddress.Loopback, port);
            var tcpListener = new TcpListener(nodeIpEndpoint);
            tcpListener.Start();
            return Task.Run(async () => {
                await function(tcpListener);
            });
        }

        private Task Listen(TcpListener tcpListener) {
            return Task.Run(async () => {
                await tcpListener.AcceptTcpClientAsync();
            });
        }

        private Task SendMessage(TcpListener tcpListener) {
            return Task.Run(async () => {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                tcpClient.GetStream().Write(_bytesToSend, 0, _bytesToSend.Length);
            });
        }

        private Task RecieveMessage(TcpListener tcpListener) {
            return Task.Run(async () => {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                _bytesRecieved = new byte[_bytesToSend.Length];
                await tcpClient.GetStream().ReadAsync(_bytesRecieved, 0, _bytesRecieved.Length);
            });
        }
    }
}
