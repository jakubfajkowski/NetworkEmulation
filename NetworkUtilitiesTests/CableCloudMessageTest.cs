using System;
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

            var cableCloudMessage = new CableCloudMessage(100);
            cableCloudMessage.Fill();

            var actualBytesLength = BinarySerializer.Serialize(cableCloudMessage).Length;

            Assert.AreEqual(expectedBytesLength, actualBytesLength);
        }

        [TestMethod]
        public void ManyStaticFieldsSettersTest() {
            for (var i = 0; i < 100; i++) StaticFieldsSettersTest();
        }

        [TestMethod]
        public void GetAtmCellsTest() {
            CableCloudMessage.MaxAtmCellsNumber = _random.Next(0, 10000);
            var cableCloudMessage = new CableCloudMessage(100);
            var expected = new AtmCell(1, 1, new byte[48]);
            cableCloudMessage.Add(expected);
            cableCloudMessage.Fill();

            var actual = cableCloudMessage.AtmCells[0];
            Assert.AreEqual(1, cableCloudMessage.AtmCells.Count);
            Assert.AreEqual(expected, actual);
        }
    }
}