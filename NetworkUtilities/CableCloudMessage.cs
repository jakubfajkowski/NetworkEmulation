using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities {
    class CableCloudMessage {
        public int port { get; set; }
        public List<ATMCell> atmCells { get; private set; }

        public CableCloudMessage(int port) {
            this.port = port;
            this.atmCells = new List<ATMCell>();
        }

        public void add(ATMCell atmCell) {
            atmCells.Add(atmCell);
        }
    }
}
