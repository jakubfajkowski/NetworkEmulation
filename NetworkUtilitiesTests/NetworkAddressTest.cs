using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class NetworkAddressTest {
        [TestMethod]
        public void TestEquals() {
            var n1 = new NetworkAddress("1");
            var n2 = new NetworkAddress("1");
            var n3 = new NetworkAddress("2");
            var n4 = new NetworkAddress("1.1");

            Assert.AreEqual(n1, n2);
            Assert.AreNotEqual(n1, n3);
            Assert.AreNotEqual(n1, n4);
        }
    }
}