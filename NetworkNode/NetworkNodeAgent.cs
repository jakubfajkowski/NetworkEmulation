using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    public class NetworkNodeAgent
    {
        private CommutationTable commutationTable;


        public NetworkNodeAgent()
        {
            commutationTable = new CommutationTable();
        }

        /* Wywołuje metodę tabeli połączeń, która dodaje połączenie */
        public void addConnectionToTable(int inVPI, int inVCI, int inPortNumber,int outVPI, int outVCI, int linkNumber)
        {
            commutationTable.addConnection(inVPI, inVCI, inPortNumber, outVPI, outVCI, linkNumber);
        }

        /* Wywołuje metodę tabeli połączeń, która usuwa połączenie */
        public bool removeConnectionFromTable(int inVPI, int inVCI, int inPortNumber, int outVPI, int outVCI, int outPortNumber)
        {
            return commutationTable.removeConnection(inVPI, inVCI, inPortNumber, outVPI, outVCI, outPortNumber);
        }

        /* Getter potrzebny do tego, żeby przekazać obiekt do pola komutacyjnego (CommutationMatrix) */
        public CommutationTable getCommutationTable()
        {
            return commutationTable;
        }
    }
}
