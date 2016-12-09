using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkEmulation {
    public class SocketNodePortPair {
        public int nodePortNumber { get; }
        public int socketPortNumber { get; }

        public SocketNodePortPair(int nodePortNumber, int socketPortNumber) {
            this.nodePortNumber = nodePortNumber;
            this.socketPortNumber = socketPortNumber;
        }

        public override bool Equals(object obj) {
            var other = obj as SocketNodePortPair;
            if (other == null)
                return false;

            bool nodePortNumberIsEqual = nodePortNumber == other.nodePortNumber;
            bool socketPortNumberIsEqual = socketPortNumber == other.socketPortNumber;

            return nodePortNumberIsEqual && socketPortNumberIsEqual;
        }

        public override int GetHashCode() {
            return nodePortNumber ^ socketPortNumber;
        }
    }
}
