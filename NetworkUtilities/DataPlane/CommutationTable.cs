using System;
using System.Collections.Generic;
using NetworkUtilities.Log;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.DataPlane {
    public class CommutationTable : LogObject {
        private readonly List<CommutationTableRow> _commutationTableRows;

        public CommutationTable() {
            _commutationTableRows = new List<CommutationTableRow>();
        }

        public CommutationTableRow FindRow(int inVpi, int inVci, int inPortNumber) {
            return _commutationTableRows.Find(row => row.CheckInIdentifiers(inVpi, inVci, inPortNumber));
        }

        public void AddConnection(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci, int outPortNumber) {
            _commutationTableRows.Add(new CommutationTableRow(inVpi, inVci, inPortNumber, outVpi, outVci, outPortNumber));

            OnUpdateState("Added connection: VPI in/out: " + inVpi + "/" + outVpi +
                          ",   VCI in/out:" + inVci +
                          "/" + outVci + ",  port in/out: " + inPortNumber + "/" + outPortNumber);
        }

        public void AddConnection(CommutationTableRow c) {
            AddConnection(c._inVpi, c._inVci, c._inPortNumber, c._outVpi, c._outVci, c._outPortNumber);
        }

        public bool RemoveConnection(CommutationTableRow rowToRemove) {
            return _commutationTableRows.Remove(rowToRemove);
        }
    }
}