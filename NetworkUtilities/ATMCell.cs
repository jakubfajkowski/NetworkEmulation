using System;

namespace NetworkUtilities
{
    [Serializable]
    public class ATMCell
    {
        public int VPI { get; set; }
        public int VCI { get; set; }
        public byte[] data { get; private set; }

        public ATMCell(int vpi, int vci, byte[] data) {
            VPI = vpi;
            VCI = vci;
            this.data = data;
        }
    }
}
