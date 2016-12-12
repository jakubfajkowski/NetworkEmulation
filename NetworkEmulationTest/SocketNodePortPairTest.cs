using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation;
using NetworkEmulation.network;

namespace NetworkEmulationTest {
    [TestClass]
    public class SocketNodePortPairTest {
        public SocketNodePortPairTest() {
            _linkDictionary = new Dictionary<SocketNodePortPair, SocketNodePortPair>();
        }

        private TestContext testContextInstance;

        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        private Dictionary<SocketNodePortPair, SocketNodePortPair> _linkDictionary;


        [TestMethod]
        public void InsertRecord() {
            var pair1 = new SocketNodePortPair(1, 1);
            var pair2 = new SocketNodePortPair(2, 2);
            _linkDictionary.Add(pair1, pair2);

            Assert.AreEqual(pair2, _linkDictionary[pair1]);
        }

        [TestMethod]
        public void Equals() {
            var pair1 = new SocketNodePortPair(1, 1);
            var pair2 = new SocketNodePortPair(1, 1);

            Assert.AreEqual(pair2, pair1);
        }

        [TestMethod]
        public void GetValueByEqualObject() {
            var pair1 = new SocketNodePortPair(1, 1);
            var pair2 = new SocketNodePortPair(2, 2);
            var pair3 = new SocketNodePortPair(1, 1);

            _linkDictionary.Add(pair1, pair2);

            Assert.AreEqual(pair2, _linkDictionary[pair3]);
        }
    }
}
