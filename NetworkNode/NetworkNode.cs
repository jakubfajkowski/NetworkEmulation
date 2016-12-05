using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode
{
    /* Dodać port do łączenia z chmurą
             listę portów do łączenia z klientami (jaka jest różnica między portem klienckim a sieciowym jeśli klient przesyła przez chmurę kablową)

       Socket (ten od chmury) zawiera metodę receive(), która tworzy pakiet ATM z wczytanych X bitów, a następnie wrzuca ten pakiet do 
            bufora w CommutationMatrix (czy jeden bufor dla wszystkich czy jeden bufor dla jednego "miniportu" w polu komutacyjnym). 
            Po wrzuceniu pakietu do bufora wzbudzić wątek pola komutacyjnego (odpowiednik notify() z Javy).
            Pole komutacyjne zagląda do tablicy patrząc na VPI i VCI, usuwa je i nadaje kolejne wartości VPI, VCI 
            (kanały są jednokierunkowe, więc nie trzeba wiedzieć, od którego węzła przyszedł pakiet).
            Pole komutacyjne wrzuca przerobiony pakiet do bufora pakietów, które mają się wysłać. (Czyli chyba w Sockecie musi być bufor)
            (Socket) wysyła pakiet do chmury.

        Przy podmianie VPI VCI pamiętać że VCI może być niezmienione kiedy cała ścieżka leci dalej.
    */
    class NetworkNode
    {
        public CommutationMatrix commutationMatrix;
        public NetworkNodeAgent networkNodeAgent;

        public NetworkNode()
        {
            networkNodeAgent = new NetworkNodeAgent();
            commutationMatrix = new CommutationMatrix(networkNodeAgent.getCommutationTable());
            
        }

    }
}
