using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.ControlPlane.GraphAlgorithm;
using NetworkUtilities.Utilities;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class RoutingControllerTest {
        [TestMethod]
        public void TestNoDirectConnection() {
            var snpp1 = new SubnetworkPointPool(new NetworkAddress("1.1.1"));
            var snpp2 = new SubnetworkPointPool(new NetworkAddress("1.1.2"));
            var snpp3 = new SubnetworkPointPool(new NetworkAddress("1.2.1"));
            var snpp4 = new SubnetworkPointPool(new NetworkAddress("1.2.2"));
            var snpp5 = new SubnetworkPointPool(new NetworkAddress("1.3.1"));
            var snpp6 = new SubnetworkPointPool(new NetworkAddress("1.3.1"));

            var link13 = new Link(snpp1, snpp3);
            var link24 = new Link(snpp2, snpp4);
            var link46 = new Link(snpp4, snpp6);

            var result = Engine.CalculateShortestPathBetween(
                snpp1,
                snpp5,
                new[] {
                    new Path<SubnetworkPointPool> {Source = snpp1, Destination = snpp3, Link = link13},
                    new Path<SubnetworkPointPool> {Source = snpp2, Destination = snpp4, Link = link24},
                    new Path<SubnetworkPointPool> {Source = snpp4, Destination = snpp6, Link = link46}
                }
            );

            Assert.IsTrue(result.Count == 0);
        }
    }
}
