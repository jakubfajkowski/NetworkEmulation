﻿using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class PathComputationServerTest {
        [TestMethod]
        public void TestConnection() {
            var nameServerPort = 30000;
            var nameServer = new NameServer(nameServerPort);
            nameServer.StartListening();

            var sspcs1Address = new NetworkAddress("1");
            var sspcs1Port = PortRandomizer.RandomFreePort();
            var sspcs2Address = new NetworkAddress("2");
            var sspcs2Port = PortRandomizer.RandomFreePort();

            var hpcs1Address = new NetworkAddress("1.1");
            var hpcs1Port = PortRandomizer.RandomFreePort();
            var hpcs2Address = new NetworkAddress("1.2");
            var hpcs2Port = PortRandomizer.RandomFreePort();

            var sspcs1 = new StepByStepPathComputationServer(sspcs1Address, sspcs2Address, "127.0.0.1", sspcs1Port, sspcs2Port, nameServerPort);
            sspcs1.UpdateState += OnUpdateState;
            sspcs1.StartListening();
            var sspcs2 = new StepByStepPathComputationServer(sspcs2Address, sspcs1Address, "127.0.0.1", sspcs2Port, sspcs2Port, nameServerPort);
            sspcs2.UpdateState += OnUpdateState;
            sspcs2.StartListening();

            sspcs1.Initialize();
            sspcs2.Initialize();

            var hpcs1 = new HierarchicalPathComputationServer(hpcs1Address, "127.0.0.1", sspcs1Port, hpcs1Port);
            hpcs1.UpdateState += OnUpdateState;
            hpcs1.StartListening();
            var hpcs2 = new HierarchicalPathComputationServer(hpcs2Address, "127.0.0.1", sspcs1Port, hpcs2Port);
            hpcs2.UpdateState += OnUpdateState;
            hpcs2.StartListening();

            hpcs1.Initialize();
            hpcs2.Initialize();

            Thread.Sleep(5000);
        }

        private void OnUpdateState(object sender, string state) {
            Console.WriteLine(state);
        }
    }
}