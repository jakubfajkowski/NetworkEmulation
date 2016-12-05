using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities
{
    public class ATMCell
    {
        public int VPI { get; private set; }
        public int VCI { get; private set; }
        public byte[] data { get; private set; }

        public ATMCell(int vpi, int vci, byte[] data) {
            VPI = vpi;
            VCI = vci;
            this.data = data;
        }
    }
}
