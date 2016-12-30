using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class CableCloudMessageTest {
        private readonly Random _random = new Random();

        [TestMethod]
        public void StaticFieldsSettersTest() {
            CableCloudMessage.MaxAtmCellsNumber = _random.Next(0, 10000);
            var expectedBytesLength = CableCloudMessage.MaxByteBufferSize;

            var atmCells = new List<AtmCell>();
            for (var i = 0; i < CableCloudMessage.MaxAtmCellsNumber; i++) atmCells.Add(new AtmCell());

            var cableCloudMessage = new CableCloudMessage(100, atmCells);

            var actualBytesLength = cableCloudMessage.Serialize().Length;

            Assert.AreEqual(expectedBytesLength, actualBytesLength);
        }

        [TestMethod]
        public void ManyStaticFieldsSettersTest() {
            for (var i = 0; i < 100; i++) StaticFieldsSettersTest();
        }
    }
}