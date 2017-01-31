using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities.DataPlane;
using NetworkUtilities.Utilities;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkEmulationTest {
    [TestClass]
    public class CableCloudTest {
        private readonly IPEndPoint _cableCloudIpEndpoint;

        private readonly Random _random;
        private byte[] _bytesReceived;
        private byte[] _bytesToSend;

        public CableCloudTest() {
            _random = new Random();
            _cableCloudIpEndpoint = new IPEndPoint(IPAddress.Loopback, 10000);
        }

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CableCloudBindEndpointTest() {
            var cableCloud = new CableCloud(10000);
            cableCloud.StartListening();
            var port = 10001;

            var listenerTask = StartTcpListener(port, Listen);

            ConnectToCableCloud(port);
            listenerTask.Wait();

            var nodesTcpClients =
                (Dictionary<int, TcpClient>) new PrivateObject(cableCloud).GetField("NodesTcpClients");
            Assert.AreEqual(1, nodesTcpClients.Count);
        }

        [TestMethod]
        public void CableCloudPassMessageTest() {
            var cableCloud = new CableCloud(10000);
            cableCloud.UpdateState += (sender, state) => Console.WriteLine(state);
            cableCloud.StartListening();

            var port1 = 10001;
            var port2 = 10002;
            var port3 = 10003;

            var address1 = new NetworkAddress(1);
            var address2 = new NetworkAddress(2);
            var address3 = new NetworkAddress(3);

            var output = new NetworkAddressNodePortPair(address1, 1);
            var input1 = new NetworkAddressNodePortPair(address2, 1);
            var input2 = new NetworkAddressNodePortPair(address3, 1);

            cableCloud.AddLink(input1, output);
            cableCloud.AddLink(input2, output);
            _bytesToSend = BinarySerializer.Serialize(CreateCableCloudMessage(1, 100));

            var listenerTask1 = StartTcpListener(port1, ReceiveMessage);
            ConnectToCableCloud(port1);
            var listenerTask2 = StartTcpListener(port2, SendMessage);
            ConnectToCableCloud(port2);
            var listenerTask3 = StartTcpListener(port3, SendMessage);
            ConnectToCableCloud(port3);

            Task.WaitAll(listenerTask1, listenerTask2, listenerTask3);

            for (var i = 0; i < _bytesToSend.Length; i++) Assert.AreEqual(_bytesToSend[i], _bytesReceived[i]);
        }

        private static CableCloudMessage CreateCableCloudMessage(int linkNumber, int atmCellsNumber) {
            var atmCells = new List<AtmCell>();
            for (var i = 0; i < atmCellsNumber; i++) atmCells.Add(new AtmCell(1, 1, new byte[48]));

            return new CableCloudMessage(1, atmCells);
        }

        private NetworkAddressNodePortPair RandomNetworkAddressNodePortPair() {
            var networkAddress = new NetworkAddress(_random.Next());
            return new NetworkAddressNodePortPair(networkAddress, _random.Next());
        }

        private void ConnectToCableCloud(int port) {
            var udpClient = new UdpClient();

            var bytesToSend = BitConverter.GetBytes(port);
            udpClient.Send(bytesToSend, bytesToSend.Length, _cableCloudIpEndpoint);
        }

        private Task StartTcpListener(int port, Func<TcpListener, Task> function) {
            var nodeIpEndpoint = new IPEndPoint(IPAddress.Loopback, port);
            var tcpListener = new TcpListener(nodeIpEndpoint);
            tcpListener.Start();
            return Task.Run(async () => { await function(tcpListener); });
        }

        private Task Listen(TcpListener tcpListener) {
            return Task.Run(async () => { await tcpListener.AcceptTcpClientAsync(); });
        }

        private Task SendMessage(TcpListener tcpListener) {
            return Task.Run(async () => {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                tcpClient.GetStream().Write(_bytesToSend, 0, _bytesToSend.Length);
            });
        }

        private Task ReceiveMessage(TcpListener tcpListener) {
            return Task.Run(async () => {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                _bytesReceived = new byte[_bytesToSend.Length];
                await tcpClient.GetStream().ReadAsync(_bytesReceived, 0, _bytesReceived.Length);
            });
        }

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
    }
}