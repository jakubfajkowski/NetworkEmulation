using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities {
    [Serializable]
    public class CableCloudMessage {
        public int linkNumber { get; set; }
        public List<ATMCell> atmCells { get; }

        public CableCloudMessage(int linkNumber) {
            this.linkNumber = linkNumber;
            this.atmCells = new List<ATMCell>();
        }

        public void add(ATMCell atmCell) {
            atmCells.Add(atmCell);
        }
    }
}
