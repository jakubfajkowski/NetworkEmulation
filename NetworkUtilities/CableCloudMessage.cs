using System;
using System.Collections.Generic;
using NetworkUtilities.Serialization;

namespace NetworkUtilities {
    [Serializable]
    public class CableCloudMessage {
        public static int MaxAtmCellsNumber = 100;

        public CableCloudMessage(int portNumber, byte[] data) {
            PortNumber = portNumber;
            Data = data;
        }

        public CableCloudMessage(int portNumber, List<AtmCell> atmCells) {
            PortNumber = portNumber;
            Data = BinarySerializer.Serialize(Fill(atmCells));
        }

        public byte[] Data { get; private set; }

        public int PortNumber { get; set; }


        private static List<AtmCell> Fill(List<AtmCell> atmCells) {
            var result = new List<AtmCell>(atmCells);

            while (result.Count < MaxAtmCellsNumber) result.Add(new AtmCell());

            return result;
        }
    }
}