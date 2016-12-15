using System;
using System.Collections.Generic;
using NetworkUtilities;

namespace NetworkNode {
    public class Port {
        private DateTime _lastAddTime;

        private readonly List<AtmCell> _portBuffer;
        /* Globalny numer łącza rzeczywistego podłączonego do danego portu */
        private readonly int _portNumber;


        public Port(int portNumber) {
            this._portNumber = portNumber;
            _portBuffer = new List<AtmCell>();
            _lastAddTime = DateTime.Now;
        }

        public AtmCell GetAtmCell() {
            if (_portBuffer.Count > 0) {
                var cell = _portBuffer[0];
                _portBuffer.Remove(cell);
                return cell;
            }
            return null;
        }

        public void AddAtmCell(AtmCell cell) {
            _portBuffer.Add(cell);
            _lastAddTime = DateTime.Now;
            //Console.WriteLine("Dodanie ATMCell do bufora wejsciowego/wyjsciowego, liczba komórek w buforze: " + portBuffer.Count);
        }

        public int GetPortNumber() {
            return _portNumber;
        }

        public DateTime GetLastAddTime() {
            return _lastAddTime;
        }

        public int GetAtmCellNumber() {
            return _portBuffer.Count;
        }
    }
}