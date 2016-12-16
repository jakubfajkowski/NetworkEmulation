namespace NetworkNode {
    public class CommutationTableRow {
        private readonly int _inPortNumber;
        private readonly int _inVci;
        private readonly int _inVpi;
        private readonly int _outPortNumber;
        private readonly int _outVci; // Jeśli wpis nie zawiera VCI to VCI = -1
        private readonly int _outVpi;

        public CommutationTableRow(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci, int outPortNumber) {
            this._inVci = inVci;
            this._inVpi = inVpi;
            this._inPortNumber = inPortNumber;
            this._outVci = outVci;
            this._outVpi = outVpi;
            this._outPortNumber = outPortNumber;
        }

        /* Metoda sprawdzająca czy podane wejściowe VPI i VCI pasują do danego wpisu
            Wywoływana przy wyszukiwaniu wyjściowych VPI i VCI */

        public bool CheckInIdentifiers(int inVpi, int inVci, int inPortNumber) {
            return (inPortNumber == this._inPortNumber) && (inVpi == this._inVpi) &&
                   ((inVci == this._inVci) || (inVci == -1));
        }

        /* Metoda sprawdzająca czy podane VPI i VCI pasują do danego wpisu
            Wywoływana przy usuwaniu połączenia z tabeli połączeń */

        public bool CheckAllIdentifiers(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci,
            int outPortNumber) {
            if ((this._inVci == inVci) && (this._inVpi == inVpi) && (this._inPortNumber == inPortNumber) &&
                (this._outVci == outVci) && (this._outVpi == outVpi) && (this._outPortNumber == outPortNumber))
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