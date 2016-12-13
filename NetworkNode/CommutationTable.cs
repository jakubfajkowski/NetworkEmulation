using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    public class CommutationTable
    {
        private List<CommutationTableRow> commutationTableRows;

        public CommutationTable()
        {
            commutationTableRows = new List<CommutationTableRow>();
        }


        /* Metoda używana do znalezienia wyjściowych VPI i VCI
            Sprawdza wszystkie wiersze w tablicy i zwraca ten, który zawiera podane wejściowe VPI i VCI
            Jeśli wiersz nie zostanie znaleziony zwraca null */
        public CommutationTableRow check(int inVPI, int inVCI, int inPortNumber)
        {
            for (int i = 0; i < commutationTableRows.Count; i++)
            {
                if (commutationTableRows[i].checkInIdentifiers(inVPI, inVCI, inPortNumber))
                {
                    return commutationTableRows[i];
                }
            }
            return null;
        }

        // Dodaje połączenie do tabeli połączeń i wypisuje informację na ekranie.
        public void addConnection(int inVPI, int inVCI, int inPortNumber, int outVPI, int outVCI, int outPortNumber)
        {
            commutationTableRows.Add(new CommutationTableRow(inVPI, inVCI, inPortNumber, outVPI, outVCI, outPortNumber));
            Console.Out.WriteLine("Dodano połączenie: VPI in/out: " + inVPI + "/" + outVPI + "   VCI in/out:" + inVCI + "/" + outVCI + "   port in/out: " + inPortNumber+"/"+outPortNumber);
        }

        /* Metoda usuwająca wpis z tabeli połączeń */
        public bool removeConnection(int inVPI, int inVCI, int inPortNumber, int outVPI, int outVCI, int outPortNumber)
        {
            for (int i=0; i< commutationTableRows.Count; i++)
            {
                if (commutationTableRows[i].checkAllIdentifiers(inVPI, inVCI, inPortNumber, outVPI, outVCI, outPortNumber))
                {
                    commutationTableRows.RemoveAt(i);
                    //Console.Out.WriteLine("Usunięto połączenie");
                    return true;
                }                  
            }
            //Console.Out.WriteLine("Nie znaleziono polaczenia");
            return false;
        }
    }
}
