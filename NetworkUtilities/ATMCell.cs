using System;

namespace NetworkUtilities {
    [Serializable]
    public class AtmCell {
        public AtmCell(int vpi, int vci, byte[] data) {
            Vpi = vpi;
            Vci = vci;
            Data = data;
        }

        public int Vpi { get; set; }
        public int Vci { get; set; }
        public byte[] Data { get; private set; }
    }
}