using System;
using System.Collections.Generic;

namespace NetworkNode {
    public class CommutationTable {
        private readonly List<CommutationTableRow> _commutationTableRows;

        public CommutationTable() {
            _commutationTableRows = new List<CommutationTableRow>();
        }


        /* Metoda używana do znalezienia wyjściowych VPI i VCI
            Sprawdza wszystkie wiersze w tablicy i zwraca ten, który zawiera podane wejściowe VPI i VCI
            Jeśli wiersz nie zostanie znaleziony zwraca null */

        public CommutationTableRow Check(int inVpi, int inVci, int inPortNumber) {
            for (var i = 0; i < _commutationTableRows.Count; i++)
                if (_commutationTableRows[i].CheckInIdentifiers(inVpi, inVci, inPortNumber))
                    return _commutationTableRows[i];
            return null;
        }

        // Dodaje połączenie do tabeli połączeń i wypisuje informację na ekranie.
        public void AddConnection(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci, int outPortNumber) {
            _commutationTableRows.Add(new CommutationTableRow(inVpi, inVci, inPortNumber, outVpi, outVci, outPortNumber));
            Console.Out.WriteLine("Dodano połączenie: VPI in/out: " + inVpi + "/" + outVpi + "   VCI in/out:" + inVci +
                                  "/" + outVci + "   port in/out: " + inPortNumber + "/" + outPortNumber);
        }

        /* Metoda usuwająca wpis z tabeli połączeń */

        public bool RemoveConnection(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci, int outPortNumber) {
            for (var i = 0; i < _commutationTableRows.Count; i++)
                if (_commutationTableRows[i].CheckAllIdentifiers(inVpi, inVci, inPortNumber, outVpi, outVci,
                    outPortNumber)) {
                    _commutationTableRows.RemoveAt(i);
                    //Console.Out.WriteLine("Usunięto połączenie");
                    return true;
                }
            //Console.Out.WriteLine("Nie znaleziono polaczenia");
            return false;
        }
    }
}