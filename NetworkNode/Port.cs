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
            if (_atmCells.Count > 0)
                return _atmCells.Dequeue();
            else return null;
        }

        public void AddAtmCell(AtmCell cell) {
            _lastAddTime = DateTime.Now;
            _atmCells.Enqueue(cell);      
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