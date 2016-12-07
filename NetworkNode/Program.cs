﻿using NetworkUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkNode {
    class Program {

        static void Main(string[] args) {

            

            

            
            NetworkNode networkNode = new NetworkNode(10000,10001);
            //Test1(networkNode);

            
            Console.In.ReadLine();
        }



        static void Test1(NetworkNode networkNode)
        {
            networkNode.networkNodeAgent.addConnectionToTable(1, 2, 3, 4, 10);
            networkNode.networkNodeAgent.addConnectionToTable(4, 42, 33, 2, 13);
            networkNode.networkNodeAgent.addConnectionToTable(3, 90, 2, 33, 46);
            Console.WriteLine();

            Console.Write("Wyszukiwanie polaczenia (powinno sie udac):    ");
            if (networkNode.commutationMatrix.Wypisz())
                Console.WriteLine("OK");
            else Console.WriteLine("Błąd");
            
         /*   Console.Write("Usuwanie polaczenia (powinno sie udac):    ");
            if (networkNode.networkNodeAgent.removeConnectionFromTable(1, 2, 3, 4))
                Console.WriteLine("OK");
            else Console.WriteLine("Błąd"); */

            Console.Write("Wyszukiwanie usuniętego polaczenia (powinno sie nie udac):    ");
            if (networkNode.commutationMatrix.Wypisz())
                Console.WriteLine("Błąd");
            else Console.WriteLine("OK");

            Console.Write("Dodawanie portów...        ");
            networkNode.commutationMatrix.createInputPort(11);
            networkNode.commutationMatrix.createInputPort(23);
            networkNode.commutationMatrix.createInputPort(31);
            networkNode.commutationMatrix.createOutputPort(10);
            networkNode.commutationMatrix.createOutputPort(13);
            networkNode.commutationMatrix.createOutputPort(46);
            Console.WriteLine("OK");

            /*   Console.WriteLine("Dodawanie komorki ATM");
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
               Thread.Sleep(500);
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 23);
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 23);
               Thread.Sleep(500);
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 11);
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 31);
               networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 23);
                 Thread.Sleep(3000);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 31);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 11);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 Thread.Sleep(500);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 23);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 23);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 23);
                 Thread.Sleep(500);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 Thread.Sleep(5000);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 23);
                 Thread.Sleep(500);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 Thread.Sleep(500);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 Thread.Sleep(500);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null), 11);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 23);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 23);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(3, 90, null), 31);
                 networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(1, 2, null), 31);
               */

            CableCloudMessage message = new CableCloudMessage(23);
            for (int i=0;i<32;i++)
            {
                message.add(new ATMCell(4, 42, null));
                message.add(new ATMCell(3, 90, null));
                message.add(new ATMCell(4, 42, null));
                message.add(new ATMCell(1, 2, null));
            }
            networkNode.receiveCableCloudMessage(message);
            message = new CableCloudMessage(11);
            Thread.Sleep(10000);
            for (int i = 0; i < 32; i++)
            {
                message.add(new ATMCell(4, 42, null));
                message.add(new ATMCell(3, 90, null));
                message.add(new ATMCell(4, 42, null));
                message.add(new ATMCell(1, 2, null));
            }
            networkNode.receiveCableCloudMessage(message);
            
              
            //for (int i=0; i<100;i++)
            //networkNode.commutationMatrix.addATMCellToInputPort(new ATMCell(4, 42, null));

        }



    }
}

/*
TU DU:
- W NetworkNode zmienić pola z public na private
*/
