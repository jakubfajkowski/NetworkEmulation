namespace NetworkUtilities.Utilities {
    public class CommutationTableRow {
        public int _inPortNumber;
        public int _inVci;
        public int _inVpi;
        public readonly int _outPortNumber;
        public readonly int _outVci; // Jeśli wpis nie zawiera VCI to VCI = -1
        public readonly int _outVpi;

        public CommutationTableRow(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci, int outPortNumber) {
            _inVci = inVci;
            _inVpi = inVpi;
            _inPortNumber = inPortNumber;
            _outVci = outVci;
            _outVpi = outVpi;
            _outPortNumber = outPortNumber;
        }

        /* Metoda sprawdzająca czy podane wejściowe VPI i VCI pasują do danego wpisu
            Wywoływana przy wyszukiwaniu wyjściowych VPI i VCI */

        public bool CheckInIdentifiers(int inVpi, int inVci, int inPortNumber) {
            return inPortNumber == _inPortNumber && inVpi == _inVpi &&
                   (inVci == _inVci || _inVci == -1);
        }

        /* Metoda sprawdzająca czy podane VPI i VCI pasują do danego wpisu
            Wywoływana przy usuwaniu połączenia z tabeli połączeń */

        public bool CheckAllIdentifiers(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci,
            int outPortNumber) {
            if (_inVci == inVci && _inVpi == inVpi && _inPortNumber == inPortNumber &&
                _outVci == outVci && _outVpi == outVpi && _outPortNumber == outPortNumber)
                return true;
            return false;
        }

        // Getter wywoływany w celu zmiany wartości VPI w komórce ATMCell przed wysłaniem jej do kolejnego węzła
        public int GetOutVpi() {
            return _outVpi;
        }

        // Getter wywoływany w celu zmiany wartości VCI w komórce ATMCell przed wysłaniem jej do kolejnego węzła
        public int GetOutVci() {
            return _outVci;
        }

        public int GetOutPort() {
            return _outPortNumber;
        }

        public override bool Equals(object obj) {
            var other = obj as CommutationTableRow;
            if (other == null)
                return false;

            return _inPortNumber == other._inPortNumber &&
                   _inVci == other._inVci &&
                   _inVpi == other._inVpi &&
                   _outPortNumber == other._outPortNumber &&
                   _outVci == other._outVci &&
                   _outVpi == other._outVpi;
        }

        public override int GetHashCode() {
            return _inPortNumber ^
                   _inVci ^
                   _inVpi ^
                   _outPortNumber ^
                   _outVci ^
                   _outVpi;
        }
    }
}