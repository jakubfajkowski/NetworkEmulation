using System.Collections.Generic;
using System.Threading;
using NetworkUtilities.Log;

namespace NetworkUtilities.DataPlane {
    public class CommutationMatrix : LogObject {
        private readonly List<Port> _inputPorts;
        public readonly List<Port> OutputPorts;

        private bool _commuted;
        private Thread _matrixThread;

        private bool _timeToQuit;


        public CommutationMatrix(CommutationTable comTable, int portNumber) {
            _commutationTable = comTable;
            _commutationTable.UpdateState += (sender, state) => OnUpdateState(state);

            _inputPorts = new List<Port>();
            OutputPorts = new List<Port>();

            for (var i = 1; i <= portNumber; i++) {
                CreateInputPort(i);
                CreateOutputPort(i);
            }
        }

        // Tablica połączeń in/out ta sama, która się znajduje w NetworkNodeAgent
        public CommutationTable _commutationTable { get; set; }

        public void StartThread() {
            _timeToQuit = false;
            _matrixThread = new Thread(RunThread);
            _matrixThread.Start();
        }

        public void Shutdown() {
            _timeToQuit = true;
            lock (_matrixThread) {
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
            foreach (var inPort in _inputPorts)
                if (portNumber == inPort.GetPortNumber()) {
                    inPort.AddAtmCell(cell);
                    lock (_matrixThread) {
                        Monitor.Pulse(_matrixThread);
                    }

                    return;
                }
        }

        public CableCloudMessage CommuteAllCells(List<AtmCell> cells, int inputPortNumber) {
            var row = _commutationTable.FindRow(cells[0].Vpi, cells[0].Vci, inputPortNumber);
            if (row == null) return null;
            foreach (var cell in cells) {
                cell.Vpi = row.GetOutVpi();
                if (row.GetOutVci() != -1)
                    cell.Vci = row.GetOutVci();
            }
            return new CableCloudMessage(row.GetOutPort(), cells);
        }

        /* Metoda zmieniająca VPI, VCI na podstawie tabeli */

        public bool Commute(AtmCell cell, int inPortNumber) {
            var row = _commutationTable.FindRow(cell.Vpi, cell.Vci, inPortNumber);
            if (row != null) {
                //Console.Write("["+DateTime.Now + "] Changed inVpi: " + cell.Vpi + " inVci: "+cell.Vci);
                cell.Vpi = row.GetOutVpi();
                if (row.GetOutVci() != -1)
                    cell.Vci = row.GetOutVci();

                //OnUpdateState(" outVpi: " + cell.Vpi + " outVci: " + cell.Vci);
                return AddAtmCellToOutputPort(cell, row.GetOutPort());
            }
            //else
            // OnUpdateState("WYWALILO BLAD");
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
            if (isFree)
                ports.Add(new Port(portNumber));

            return isFree;
        }
    }
}