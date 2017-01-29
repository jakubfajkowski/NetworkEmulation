using System;
using System.Collections.Generic;
using System.Text;
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

        public byte[] Data { get; }

        public int PortNumber { get; set; }


        private static List<AtmCell> Fill(List<AtmCell> atmCells) {
            var result = new List<AtmCell>(atmCells);

            while (result.Count < MaxAtmCellsNumber) result.Add(new AtmCell());

            return result;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var cell in ExtractAtmCells()) sb.Append(Encoding.UTF8.GetString(cell.Data));
            return sb.ToString();
        }

        public List<AtmCell> ExtractAtmCells() {
            var atmCells = BinarySerializer.Deserialize(Data) as List<AtmCell>;
            return atmCells?.FindAll(cell => cell.Valid());
        }
    }
}