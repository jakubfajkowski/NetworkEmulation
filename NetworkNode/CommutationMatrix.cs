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
        private Thread _matrixThread;
        public readonly List<Port> OutputPorts;

        private bool _timeToQuit;


        public CommutationMatrix(CommutationTable comTable, int portNumber) {
            _commutationTable = comTable;
            _inputPorts = new List<Port>();
            OutputPorts = new List<Port>();

            for (int i = 1; i <= portNumber; i++) 
            {
                CreateInputPort(i);
                CreateOutputPort(i);
            } 
        }

        public void startThread()
        {
            _timeToQuit = false;
            _matrixThread = new Thread(RunThread);
            _matrixThread.Start();
        }

        public void shutdown()
        {
            _timeToQuit = true;
            lock (_matrixThread)
            {
                Monitor.Pulse(_matrixThread);
            }
        }


        private void RunThread() {
            
            while (!_timeToQuit) {
                
                _commuted = false;

                foreach (var inPort in _inputPorts) { 
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
            var row = _commutationTable.FindRow(cell.Vpi, cell.Vci, inPortNumber);
            if (row != null) {
                cell.Vpi = row.GetOutVpi();
                if (row.GetOutVci() != -1)
                    cell.Vci = row.GetOutVci();

               // Console.WriteLine("Zmiana VPI/VCI na " + cell.Vpi + "/" + cell.Vci +
                //                  " Wrzucenie komórki do portu wyjściowego o łączu " + row.GetOutPort());
                return AddAtmCellToOutputPort(cell, row.GetOutPort());
            }
            return false;
        }

        private bool AddAtmCellToOutputPort(AtmCell cell, int portNumber) {
            foreach (var outPort in OutputPorts)
                if (portNumber == outPort.GetPortNumber()) {
                    outPort.AddAtmCell(cell);
                    return true;
                }
            return false;
        }

        public bool CreateInputPort(int portNumber) {
            return CreatePort(portNumber, _inputPorts);
        }

        public bool CreateOutputPort(int portNumber) {
            return CreatePort(portNumber, OutputPorts);
        }

        public bool CreatePort(int portNumber, List<Port> ports) {
            var isFree = true;
            foreach (var port in ports)
                if (port.GetPortNumber() == portNumber)
                    isFree = false;
            if (isFree) {
                ports.Add(new Port(portNumber));
                Console.WriteLine("Port " + portNumber + " has been created.");
            }
            else
                Console.WriteLine("Port " + portNumber + " is already used.");
            return isFree;
        }
    }
}