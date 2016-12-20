using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkUtilities {
    [Serializable]
    public class AtmCell {
        public AtmCell(int vpi, int vci, byte[] data) {
            Vpi = vpi;
            Vci = vci;
            Data = data;
        }

        public AtmCell() {
            Vpi = 0;
            Vci = 0;
            Data = new byte[48];
        }

        public int Vpi { get; set; }
        public int Vci { get; set; }
        public byte[] Data { get; private set; }

        public bool Valid() {
            return Vpi != 0;
        }

        public static List<AtmCell> Generate(int vpi, int vci, string message) {
            var source = Encoding.UTF8.GetBytes(message);
            var atmCells = new List<AtmCell>();

            for (var i = 0; i < source.Length; i += 48) {
                var buffer = new byte[48];
                if (i <= source.Length - 48) {
                    Buffer.BlockCopy(source, i, buffer, 0, 48);
                    atmCells.Add(new AtmCell(vpi, vci, buffer));
                }
                else
                // gdy długość wiadomości jest mniejsza od 48 bitów, komórka jest wypełniana '0' na pozostałych miejscach
                {
                    Buffer.BlockCopy(source, i, buffer, 0, source.Length - i);
                    atmCells.Add(new AtmCell(vpi, vci, buffer));
                }
            }

            return atmCells;
        }
    }
}