using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    class CommutationMatrix
    {
        // Tablica połączeń in/out ta sama, która się znajduje w NetworkNodeAgent
        private CommutationTable commutationTable;


        public CommutationMatrix(CommutationTable comTable)
        {
            commutationTable = comTable;
        }


        //Test
        public Boolean Wypisz()
        {
            CommutationTableRow row = (commutationTable.check(1, 2));
            if (row == null)
                return false;
            else
                return true;
        }
    }
}
