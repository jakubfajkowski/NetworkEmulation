using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    public class CommutationTableRow
    {
        private int inVPI;
        private int inVCI;
        private int inPortNumber;
        private int outVPI;
        // Jeśli wpis nie zawiera VCI to VCI = -1
        private int outVCI;
        private int outPortNumber;

        public CommutationTableRow(int inVPI, int inVCI, int outVPI, int outVCI, int outPortNumber)
        {
            this.inVCI = inVCI;
            this.inVPI = inVPI;
            this.outVCI = outVCI;
            this.outVPI = outVPI;
            this.outPortNumber = outPortNumber;
        }

        /* Metoda sprawdzająca czy podane wejściowe VPI i VCI pasują do danego wpisu
            Wywoływana przy wyszukiwaniu wyjściowych VPI i VCI */
        public bool checkInIdentifiers(int inVPI, int inVCI)
        {
            return ((inVPI == this.inVPI) && ((inVCI == this.inVCI) || (inVCI == -1)));     
        }

        /* Metoda sprawdzająca czy podane VPI i VCI pasują do danego wpisu
            Wywoływana przy usuwaniu połączenia z tabeli połączeń */
        public bool checkAllIdentifiers(int inVPI, int inVCI, int outVPI, int outVCI)
        {
            if ((this.inVCI == inVCI) && (this.inVPI == inVPI) && (this.outVCI == outVCI) && (this.outVPI == outVPI))
                return true;
            else return false;
        }

        // Getter wywoływany w celu zmiany wartości VPI w komórce ATMCell przed wysłaniem jej do kolejnego węzła
        public int getOutVPI()
        {
            return outVPI;
        }
        // Getter wywoływany w celu zmiany wartości VCI w komórce ATMCell przed wysłaniem jej do kolejnego węzła
        public int getOutVCI()
        {
            return outVCI;
        }
        public int getOutPort()
        {
            return outPortNumber;
        }


    }
}
