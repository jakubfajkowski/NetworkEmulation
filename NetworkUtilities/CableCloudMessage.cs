using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkUtilities {
    [Serializable]
    public class CableCloudMessage {
        private static int _maxAtmCellsNumber = 100;
        private readonly List<AtmCell> _atmCells;

        public CableCloudMessage(int portNumber) {
            PortNumber = portNumber;
            _atmCells = new List<AtmCell>(_maxAtmCellsNumber);
        }

        private CableCloudMessage(int portNumber, List<AtmCell> atmCells) {
            PortNumber = portNumber;
            _atmCells = atmCells;
            Fill();
        }

        public static int MaxByteBufferSize { get; private set; } = 9204;

        public static int MaxAtmCellsNumber {
            get { return _maxAtmCellsNumber; }
            set {
                _maxAtmCellsNumber = value;

                var exampleCloudMessage = new CableCloudMessage(0);
                MaxByteBufferSize = exampleCloudMessage.Serialize().Length;
            }
        }

        public int PortNumber { get; set; }

        public List<AtmCell> AtmCells {
            get { return _atmCells.FindAll(cell => cell.Valid()); }
        }

        public void Add(AtmCell atmCell) {
            _atmCells.Add(atmCell);
        }

        public void Fill() {
            while (_atmCells.Count < _maxAtmCellsNumber) _atmCells.Add(new AtmCell());
        }

        public static List<CableCloudMessage> Generate(int portNumber, int vpi, int vci, string message) {
            var atmCells = AtmCell.Generate(vpi, vci, message);
            var cableCloudMessages = new List<CableCloudMessage>();

            while (atmCells.Count >= MaxAtmCellsNumber) {
                var atmCellsPart = atmCells.GetRange(0, MaxAtmCellsNumber);
                atmCells.RemoveRange(0, MaxAtmCellsNumber);
                cableCloudMessages.Add(new CableCloudMessage(portNumber, atmCellsPart));
            }
            cableCloudMessages.Add(new CableCloudMessage(portNumber, atmCells));

            return cableCloudMessages;
        }

        public byte[] Serialize() {
            return BinarySerializer.Serialize(this);
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