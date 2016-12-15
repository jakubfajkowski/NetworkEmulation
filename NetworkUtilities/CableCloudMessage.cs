using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkUtilities {
    [Serializable]
    public class CableCloudMessage {
        public const int MaxByteBufferSize = 9206;

        public CableCloudMessage(int portNumber) {
            PortNumber = portNumber;
            AtmCells = new List<AtmCell>(100);
        }

        public CableCloudMessage(int portNumber, int vpi, int vci, string message) : this(portNumber) {
            var source = Encoding.UTF8.GetBytes(message);

            for (var i = 0; i < source.Length; i += 48) {
                var buffer = new byte[48];
                if (i <= source.Length - 48) {
                    Buffer.BlockCopy(source, i, buffer, 0, 48);
                    AtmCells.Add(new AtmCell(vpi, vci, buffer));
                }
                else
                    // gdy długość wiadomości jest mniejsza od 48 bitów, komórka jest wypełniana '0' na pozostałych miejscach
                {
                    Buffer.BlockCopy(source, i, buffer, 0, source.Length - i);
                    AtmCells.Add(new AtmCell(vpi, vci, buffer));
                }
            }
        }

        public int PortNumber { get; set; }
        public List<AtmCell> AtmCells { get; }

        public void Add(AtmCell atmCell) {
            AtmCells.Add(atmCell);
        }

        public static byte[] Serialize(CableCloudMessage messageToSerialize) {
            return Serializator.Serialize(messageToSerialize);
        }

        public static CableCloudMessage Deserialize(byte[] data) {
            return Serializator.Deserialize(data) as CableCloudMessage;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var cell in AtmCells) sb.Append(Encoding.UTF8.GetString(cell.Data));
            return sb.ToString();
        }
    }
}