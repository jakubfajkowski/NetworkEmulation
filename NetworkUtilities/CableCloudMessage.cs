using System;
using System.Collections.Generic;

namespace NetworkUtilities {
    [Serializable]
    public class CableCloudMessage {
        public const int MaxByteBufferSize = 9206;
        public int portNumber { get; set; }
        public List<ATMCell> atmCells { get; }

        public CableCloudMessage(int portNumber) {
            this.portNumber = portNumber;
            this.atmCells = new List<ATMCell>(100);
        }

        public void add(ATMCell atmCell) {
            atmCells.Add(atmCell);
        }

        public static byte[] serialize(CableCloudMessage messageToSerialize) {
            return Serializator.Serialize(messageToSerialize);
        }

        public static CableCloudMessage deserialize(byte[] data) {
            return Serializator.Deserialize(data) as CableCloudMessage;
        }
    }
}
