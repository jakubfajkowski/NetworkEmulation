using System;
using System.Collections.Generic;
using System.Text;

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

        public CableCloudMessage(int portNumber, int vpi, int vci, string message) : this(portNumber) {
            var source = Encoding.UTF8.GetBytes(message);

            for (var i = 0; i < source.Length; i += 48) {
                var buffer = new byte[48];
                if (i <= source.Length - 48) {
                    Buffer.BlockCopy(source, i, buffer, 0, 48);
                    atmCells.Add(new ATMCell(vpi, vci, buffer));
                }
                else
                // gdy długość wiadomości jest mniejsza od 48 bitów, komórka jest wypełniana '0' na pozostałych miejscach
                {
                    Buffer.BlockCopy(source, i, buffer, 0, source.Length - i);
                    atmCells.Add(new ATMCell(vpi, vci, buffer));
                }
            }
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

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var cell in atmCells) sb.Append(Encoding.UTF8.GetString(cell.data));
            return sb.ToString();
        }
    }
}
