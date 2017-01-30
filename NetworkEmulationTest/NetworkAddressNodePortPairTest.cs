using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.Utilities;

namespace NetworkEmulationTest {
    [TestClass]
    public class NetworkAddressNodePortPairTest {
        private readonly Dictionary<NetworkAddressNodePortPair, NetworkAddressNodePortPair> _linkDictionary;

        public NetworkAddressNodePortPairTest() {
            _linkDictionary = new Dictionary<NetworkAddressNodePortPair, NetworkAddressNodePortPair>();
        }

        public TestContext TestContext { get; set; }


        [TestMethod]
        public void InsertRecord() {
            var pair1 = new NetworkAddressNodePortPair(new NetworkAddress(1), 1);
            var pair2 = new NetworkAddressNodePortPair(new NetworkAddress(2), 2);
            _linkDictionary.Add(pair1, pair2);

            Assert.AreEqual(pair2, _linkDictionary[pair1]);
        }

        [TestMethod]
        public void Equals() {
            var pair1 = new NetworkAddressNodePortPair(new NetworkAddress(1), 1);
            var pair2 = new NetworkAddressNodePortPair(new NetworkAddress(1), 1);

            Assert.AreEqual(pair2, pair1);
        }

        [TestMethod]
        public void GetValueByEqualObject() {
            var pair1 = new NetworkAddressNodePortPair(new NetworkAddress(1), 1);
            var pair2 = new NetworkAddressNodePortPair(new NetworkAddress(2), 2);
            var pair3 = new NetworkAddressNodePortPair(new NetworkAddress(1), 1);

            _linkDictionary.Add(pair1, pair2);

            Assert.AreEqual(pair2, _linkDictionary[pair3]);
        }
    }
}