using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    class NetworkNodeAgent
    {
        private CommutationTable commutationTable;


        public NetworkNodeAgent()
        {
            commutationTable = new CommutationTable();
        }

        /* Wywołuje metodę tabeli połączeń, która dodaje połączenie */
        public void addConnectionToTable(int inVPI, int inVCI, int outVPI, int outVCI)
        {
            commutationTable.addConnection(inVPI, inVCI, outVPI, outVCI);
        }

        /* Wywołuje metodę tabeli połączeń, która usuwa połączenie */
        public Boolean removeConnectionFromTable(int inVPI, int inVCI, int outVPI, int outVCI)
        {
            return commutationTable.removeConnection(inVPI, inVCI, outVPI, outVCI);
        }

        /* Getter potrzebny do tego, żeby przekazać obiekt do pola komutacyjnego (CommutationMatrix) */
        public CommutationTable getCommutationTable()
        {
            return commutationTable;
        }
    }
}
