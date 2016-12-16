using System;
using System.Collections.Generic;

namespace NetworkNode {
    public class CommutationTable {
        private readonly List<CommutationTableRow> _commutationTableRows;

        public CommutationTable() {
            _commutationTableRows = new List<CommutationTableRow>();
        }

        public CommutationTableRow FindRow(int inVpi, int inVci, int inPortNumber) {
            return _commutationTableRows.Find(row => row.CheckInIdentifiers(inVpi, inVci, inPortNumber));
        }

        public void AddConnection(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci, int outPortNumber) {
            _commutationTableRows.Add(new CommutationTableRow(inVpi, inVci, inPortNumber, outVpi, outVci, outPortNumber));

            Console.WriteLine("Added connection: VPI in/out: " + inVpi + "/" + outVpi + "   VCI in/out:" + inVci +
                                  "/" + outVci + "   port in/out: " + inPortNumber + "/" + outPortNumber);
        }

        public bool RemoveConnection(CommutationTableRow rowToRemove) {
            return _commutationTableRows.Remove(rowToRemove);
        }
    }
}