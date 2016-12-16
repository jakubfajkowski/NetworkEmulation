using System;
using System.Collections.Generic;
using NetworkUtilities;

namespace NetworkNode {
    public class Port {
        private DateTime _lastAddTime;
        private readonly Queue<AtmCell> _atmCells;
        /* Lokalny numer portu routera. */
        private readonly int _portNumber;


        public Port(int portNumber) {
            _portNumber = portNumber;
            _atmCells = new Queue<AtmCell>();
            _lastAddTime = DateTime.Now;
        }

        public AtmCell GetAtmCell() {
            return _atmCells.Dequeue();
        }

        public void AddAtmCell(AtmCell cell) {
            _atmCells.Enqueue(cell);
            _lastAddTime = DateTime.Now;
        }

        public int GetPortNumber() {
            return _portNumber;
        }

        public DateTime GetLastAddTime() {
            return _lastAddTime;
        }

        public int GetAtmCellNumber() {
            return _atmCells.Count;
        }
    }
}