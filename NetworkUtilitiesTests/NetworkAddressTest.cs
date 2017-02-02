using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Utilities;

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

        [TestMethod]
        public void TestChild() {
            var n1 = new NetworkAddress("1");
            var n2 = new NetworkAddress("1.1");

            var snpp1 = new SubnetworkPointPool(new NetworkAddress("1.1.1"));
            var snpp2 = new SubnetworkPointPool(new NetworkAddress("1.1.2"));
            var snpp3 = new SubnetworkPointPool(new NetworkAddress("1.1.1.1"));
            var snpp4 = new SubnetworkPointPool(new NetworkAddress("1.1.1.2"));

            var expected = n2;
            var actual = snpp1.NetworkNodeAddress.GetRootFromBeginning(n1.Levels + 1);
            Assert.AreEqual(expected, actual);
            actual = snpp2.NetworkNodeAddress.GetRootFromBeginning(n1.Levels + 1);
            Assert.AreEqual(expected, actual);
            actual = snpp3.NetworkNodeAddress.GetRootFromBeginning(n1.Levels + 1);
            Assert.AreEqual(expected, actual);
            actual = snpp4.NetworkNodeAddress.GetRootFromBeginning(n1.Levels + 1);
            Assert.AreEqual(expected, actual);
        }
    }
}