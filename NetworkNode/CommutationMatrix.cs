using System;
using System.Collections.Generic;
using System.Threading;
using NetworkUtilities;

namespace NetworkNode {
    public class CommutationMatrix {
        // Tablica połączeń in/out ta sama, która się znajduje w NetworkNodeAgent
        private readonly CommutationTable _commutationTable;

        private bool _commuted;

        private readonly List<Port> _inputPorts;
        private readonly Thread _matrixThread;
        //private List<ATMCell> matrixBuffer;
        /* Obiekt potrzebny do wywołania metody addATMCellToOutBuffer() klasy NetworkNode*/
        private NetworkNode _networkNode;
        private readonly List<Port> _outputPorts;

        private readonly bool _timeToQuit = false;


        public CommutationMatrix(CommutationTable comTable, NetworkNode networkNode) {
            this._networkNode = networkNode;
            _commutationTable = comTable;
            //matrixBuffer = new List<ATMCell>();
            _inputPorts = new List<Port>();
            _outputPorts = new List<Port>();

            _matrixThread = new Thread(RunThread);
            _matrixThread.Start();
        }


        private void RunThread() {
            var j = 0;
            while (!_timeToQuit) {
                //Console.WriteLine("Wywolanie run matrix: " + j++);
                _commuted = false;

                foreach (var inPort in _inputPorts) {
                    //Console.WriteLine("Wchodzi");     
                    var cell = inPort.GetAtmCell();
                    if (cell != null) {
                        Commute(cell, inPort.GetPortNumber());
                        _commuted = true;
                    }
                }
                if (!_commuted)
                    lock (_matrixThread) {
                        Monitor.Wait(_matrixThread);
                    }
            }
        }


        /* Metoda dodająca komórkę ATM do bufora pola komutacyjnego*/

        public void AddAtmCellToInputPort(AtmCell cell, int portNumber) {
            //Console.WriteLine("Wyszukiwanie portu wejściowego...");
            foreach (var inPort in _inputPorts)
                if (portNumber == inPort.GetPortNumber()) {
                    inPort.AddAtmCell(cell);
                    //Console.WriteLine("Dodanie komórki ATM do portu o łączu wejściowym " + inPort.getLinkNumber());

                    lock (_matrixThread) {
                        Monitor.Pulse(_matrixThread);
                    }

                    return;
                }
        }


        /* Metoda zmieniająca VPI, VCI na podstawie tabeli */

        public bool Commute(AtmCell cell, int inPortNumber) {
            //Console.WriteLine("tu wchodzi");
            var row = _commutationTable.Check(cell.Vpi, cell.Vci, inPortNumber);
            if (row != null) {
                cell.Vpi = row.GetOutVpi();
                if (row.GetOutVci() != -1)
                    cell.Vci = row.GetOutVci();

                Console.WriteLine("Zmiana VPI/VCI na " + cell.Vpi + "/" + cell.Vci +
                                  " Wrzucenie komórki do portu wyjściowego o łączu " + row.GetOutPort());
                return AddAtmCellToOutputPort(cell, row.GetOutPort());
            }
            return false;
        }

        private bool AddAtmCellToOutputPort(AtmCell cell, int portNumber) {
            foreach (var outPort in _outputPorts)
                if (portNumber == outPort.GetPortNumber()) {
                    outPort.AddAtmCell(cell);
                    return true;
                }
            return false;
        }

        public List<Port> GetOutputPortList() {
            return _outputPorts;
        }


        public bool CreateInputPort(int portNumber) {
            var isFree = true;
            foreach (var port in _inputPorts)
                if (port.GetPortNumber() == portNumber)
                    isFree = false;
            if (isFree) {
                _inputPorts.Add(new Port(portNumber));
                Console.WriteLine("Udalo sie utworzyc port wejsciowy " + portNumber);
            }
            else
                Console.WriteLine("Nie udalo sie utworzyc portu");
            return isFree;
        }

        public bool CreateOutputPort(int portNumber) {
            var isFree = true;
            foreach (var port in _outputPorts)
                if (port.GetPortNumber() == portNumber)
                    isFree = false;
            if (isFree) {
                _outputPorts.Add(new Port(portNumber));
                Console.WriteLine("Udalo sie utworzyc port wyjsciowy " + portNumber);
            }
            else
                Console.WriteLine("Nie udalo sie utworzyc portu");
            return isFree;
        }
    }
}