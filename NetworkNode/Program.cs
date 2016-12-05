using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode {
    class Program {

        static void Main(string[] args) {

            NetworkNode networkNode = new NetworkNode();
            //Test1(networkNode);



            Console.In.ReadLine();
        }



        static void Test1(NetworkNode networkNode)
        {
            networkNode.networkNodeAgent.addConnectionToTable(1, 2, 3, 4);
            networkNode.networkNodeAgent.addConnectionToTable(4, 42, 33, 2);
            networkNode.networkNodeAgent.addConnectionToTable(3, 90, 2, 33);
            Console.WriteLine();

            Console.Write("Wyszukiwanie polaczenia (powinno sie udac):    ");
            if (networkNode.commutationMatrix.Wypisz())
                Console.WriteLine("OK");
            else Console.WriteLine("Błąd");
            
            Console.Write("Usuwanie polaczenia (powinno sie udac):    ");
            if (networkNode.networkNodeAgent.removeConnectionFromTable(1, 2, 3, 4))
                Console.WriteLine("OK");
            else Console.WriteLine("Błąd");

            Console.Write("Wyszukiwanie usuniętego polaczenia (powinno sie nie udac):    ");
            if (networkNode.commutationMatrix.Wypisz())
                Console.WriteLine("Błąd");
            else Console.WriteLine("OK");
        }



    }
}

/*
TU DU:
- W NetworkNode zmienić pola z public na private
*/
