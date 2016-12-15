using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkEmulation.network;
using System.Threading;

namespace NetworkEmulationTest {
    [TestClass]
    public class NetworkMangmentSystemTest {
        [TestMethod]
        public void Initialize() {
            NetworkMangmentSystem nms = new NetworkMangmentSystem();
            var networkNode = new NetworkNode.NetworkNode();
            Thread.Sleep(10000);
        }
    }
}
