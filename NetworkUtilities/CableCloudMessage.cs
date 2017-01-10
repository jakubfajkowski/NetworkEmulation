using System;
using System.Collections.Generic;
using NetworkUtilities.Serialization;

namespace NetworkUtilities {
    [Serializable]
    public class CableCloudMessage {
        private static int _maxAtmCellsNumber;

        public CableCloudMessage(int portNumber, byte[] data) {
            PortNumber = portNumber;
            Data = data;
        }

        public CableCloudMessage(int portNumber, List<AtmCell> atmCells) {
            PortNumber = portNumber;
            Data = BinarySerializer.Serialize(Fill(atmCells));
        }


        private static List<AtmCell> Fill(List<AtmCell> atmCells) {
            var result = new List<AtmCell>(atmCells);

            while (result.Count < MaxAtmCellsNumber) result.Add(new AtmCell());

            return result;
        }

        public byte[] Data { get; private set; }

        public int PortNumber { get; set; }

        public static int MaxByteBufferSize { get; private set; }

        public static int MaxAtmCellsNumber {
            get { return _maxAtmCellsNumber; }
            set {
                _maxAtmCellsNumber = value;

                var atmCells = new List<AtmCell>();
                
                var exampleCloudMessage = new CableCloudMessage(0, Fill(atmCells));
                MaxByteBufferSize = exampleCloudMessage.Serialize().Length + 2;
            }
        }

        public byte[] Serialize() {
            return BinarySerializer.Serialize(this);
        }

        public static CableCloudMessage Deserialize(byte[] data) {
            return BinarySerializer.Deserialize(data) as CableCloudMessage;
        }
    }
}