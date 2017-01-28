using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class PathComputationServerTest {
        [TestMethod]
        public void TestConnection() {
            var sspcs1Address = new NetworkAddress("1");
            var sspcs1Port = PortRandomizer.RandomFreePort();
            var sspcs2Address = new NetworkAddress("2");
            var sspcs2Port = PortRandomizer.RandomFreePort();

            var hpcs1Address = new NetworkAddress("1.1");
            var hpcs1Port = PortRandomizer.RandomFreePort();
            var hpcs2Address = new NetworkAddress("1.2");
            var hpcs2Port = PortRandomizer.RandomFreePort();

            var sspcs1 = new StepByStepPathComputationServer(sspcs1Address, "127.0.0.1", 10001, 10002, sspcs1Port);
            var sspcs2 = new StepByStepPathComputationServer(sspcs2Address, "127.0.0.1", 10002, 10001, sspcs2Port);

            sspcs1.Initialize();
            sspcs2.Initialize();

            var hpcs1 = new StepByStepPathComputationServer(hpcs1Address, "127.0.0.1", 10003, 10001, hpcs1Port);
            var hpcs2 = new StepByStepPathComputationServer(hpcs2Address, "127.0.0.1", 10004, 10001, hpcs2Port);

            hpcs1.Initialize();
            hpcs2.Initialize();
        }
    }
}
