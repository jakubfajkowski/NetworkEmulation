using System.Collections.Generic;
using System.Threading;
using NetworkUtilities;
using System;

namespace NetworkNode {
    public class CommutationMatrix {
        // Tablica połączeń in/out ta sama, która się znajduje w NetworkNodeAgent
        private readonly CommutationTable _commutationTable;

        private readonly List<Port> _inputPorts;
        public readonly List<Port> OutputPorts;

        private bool _commuted;

        public CommutationMatrix(CommutationTable comTable, int portNumber) {
            _commutationTable = comTable;
            _inputPorts = new List<Port>();
            OutputPorts = new List<Port>();

            for (var i = 1; i <= portNumber; i++) {
                CreateInputPort(i);
                CreateOutputPort(i);
            }
        }

     
        public CableCloudMessage CommuteAllCells(List<AtmCell> cells, int inputPortNumber)
        {
            var row = _commutationTable.FindRow(cells[0].Vpi, cells[0].Vci, inputPortNumber);
            if (row == null)
            {
                Console.WriteLine("Nie znaleziono wpisu w tablicy!!!!!!!!!!!");
                return null;
            }
            foreach (var cell in cells)
            {
                cell.Vpi = row.GetOutVpi();
                if (row.GetOutVci() != -1)
                    cell.Vci = row.GetOutVci();
            }
            return new CableCloudMessage(row.GetOutPort(), cells);          
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