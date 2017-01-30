using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Utilities;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class PathComputationServerTest {
        [TestMethod]
        public void TestConnection() {
            var signallingCloudPort = 20000;
            var signallingCloud = new SignallingCloud(signallingCloudPort);
            var nameServer = new NameServer("127.0.0.1", signallingCloudPort);

            var sspcs1Address = new NetworkAddress("1");
            var sspcs2Address = new NetworkAddress("2");
            var hpcs1Address = new NetworkAddress("1.1");
            var hpcs2Address = new NetworkAddress("1.2");

            var sspcs1 = new StepByStepPathComputationServer(sspcs1Address, "127.0.0.1", signallingCloudPort);
            sspcs1.UpdateState += OnUpdateState;

            var sspcs2 = new StepByStepPathComputationServer(sspcs2Address, "127.0.0.1", signallingCloudPort);
            sspcs2.UpdateState += OnUpdateState;

            sspcs1.Initialize();
            sspcs2.Initialize();

            var hpcs1 = new HierarchicalPathComputationServer(hpcs1Address, "127.0.0.1", signallingCloudPort);
            hpcs1.UpdateState += OnUpdateState;

            var hpcs2 = new HierarchicalPathComputationServer(hpcs2Address, "127.0.0.1", signallingCloudPort);
            hpcs2.UpdateState += OnUpdateState;

            hpcs1.Initialize();
            hpcs2.Initialize();

            Thread.Sleep(5000);
        }

        private void OnUpdateState(object sender, string state) {
            Console.WriteLine(state);
        }
    }
}