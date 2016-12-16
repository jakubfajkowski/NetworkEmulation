using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkUtilities {
    [Serializable]
    public class CableCloudMessage {
        public static int MaxByteBufferSize { get; private set; } = 9204;
        private static int _maxAtmCellsNumber = 100;

        public static int MaxAtmCellsNumber {
            get {
                return _maxAtmCellsNumber;
            }
            set {
                _maxAtmCellsNumber = value;
                
                var exampleCloudMessage = new CableCloudMessage(1);
                exampleCloudMessage.Fill();
                MaxByteBufferSize = Serialize(exampleCloudMessage).Length;
            }
        }

        public CableCloudMessage(int portNumber) {
            PortNumber = portNumber;
            _atmCells = new List<AtmCell>(_maxAtmCellsNumber);
        }

        public CableCloudMessage(int portNumber, int vpi, int vci, string message) : this(portNumber) {
            var source = Encoding.UTF8.GetBytes(message);

            for (var i = 0; i < source.Length; i += 48) {
                var buffer = new byte[48];
                if (i <= source.Length - 48) {
                    Buffer.BlockCopy(source, i, buffer, 0, 48);
                    _atmCells.Add(new AtmCell(vpi, vci, buffer));
                }
                else
                    // gdy długość wiadomości jest mniejsza od 48 bitów, komórka jest wypełniana '0' na pozostałych miejscach
                {
                    Buffer.BlockCopy(source, i, buffer, 0, source.Length - i);
                    _atmCells.Add(new AtmCell(vpi, vci, buffer));
                }
            }
        }

        public int PortNumber { get; set; }
        private readonly List<AtmCell> _atmCells;

        public List<AtmCell> AtmCells {
            get { return _atmCells.FindAll(cell => cell.Valid()); }
        }

        public void Add(AtmCell atmCell) {
            _atmCells.Add(atmCell);
        }

        public void Fill() {
            while (_atmCells.Count < _maxAtmCellsNumber) {
                _atmCells.Add(new AtmCell());
            }
        }

        public static byte[] Serialize(CableCloudMessage messageToSerialize) {
            return BinarySerializer.Serialize(messageToSerialize);
        }

        public static CableCloudMessage Deserialize(byte[] data) {
            return BinarySerializer.Deserialize(data) as CableCloudMessage;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var cell in AtmCells) sb.Append(Encoding.UTF8.GetString(cell.Data));
            return sb.ToString();
        }
    }
}