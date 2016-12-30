﻿using System;
using System.Collections.Generic;
using System.Threading;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkNode {
    public class NetworkNode : Node {
        private Thread _networkNodeThread;

        private bool _timeToQuit;

        public CommutationMatrix CommutationMatrix;
        public NetworkNodeAgent NetworkNodeAgent;

        public NetworkNode(NetworkNodeSerializableParameters parameters)
            : base(parameters.IpAddress, parameters.CableCloudListeningPort, parameters.CableCloudDataPort) {
            CableCloudMessage.MaxAtmCellsNumber = parameters.MaxAtmCellsNumberInCableCloudMessage;
            NetworkNodeAgent.NmsPort = parameters.NetworkManagmentSystemListeningPort;
            NetworkNodeAgent = new NetworkNodeAgent(parameters.NetworkManagmentSystemDataPort, this);
            Console.WriteLine("Network Node \nNMS port: " + parameters.NetworkManagmentSystemDataPort +
                              "\nCableCloud port: " + parameters.CableCloudDataPort);
            CommutationMatrix = new CommutationMatrix(NetworkNodeAgent.GetCommutationTable(), parameters.NumberOfPorts);
            NetworkNodeAgent.SetCommutationMatrix(CommutationMatrix);

            startThread();
        }

        // Czas po jakim komórki ATM zostaną spakowane w CCM
        public static int MinLastAddTime { private get; set; } = 100;

        public void startThread() {
            //_timeToQuit = false;
            _networkNodeThread = new Thread(RunThread);
            _networkNodeThread.Start();
            CommutationMatrix.startThread();
            NetworkNodeAgent.startThread();
        }

        public void shutdown() {
            //_timeToQuit = true;
            CommutationMatrix.shutdown();
            NetworkNodeAgent.shutdown();
        }

        /* Wątek pobierający komórki ATM z portów wyjściowych pola komutacyjnego i wysyłający je do chmury kablowej */

        private void RunThread() {
            while (!_timeToQuit) {
                var sent = false;
                foreach (var port in CommutationMatrix.OutputPorts)
                    if (((port.GetAtmCellNumber() != 0) &&
                         ((DateTime.Now - port.GetLastAddTime()).TotalMilliseconds > MinLastAddTime)) ||
                        (port.GetAtmCellNumber() > CableCloudMessage.MaxAtmCellsNumber)) {
                        // Console.WriteLine("Różnica czasu: " + (DateTime.Now - port.GetLastAddTime()).TotalMilliseconds + "   if max: " + (port.GetAtmCellNumber() > CableCloudMessage.MaxAtmCellsNumber));
                        var atmCells = new List<AtmCell>();

                        var atmCellNumberInPort = port.GetAtmCellNumber();
                        int atmCellNumberInMessage;
                        if (atmCellNumberInPort > CableCloudMessage.MaxAtmCellsNumber)
                            atmCellNumberInMessage = CableCloudMessage.MaxAtmCellsNumber;
                        else
                            atmCellNumberInMessage = atmCellNumberInPort;

                        for (var i = 0; i < atmCellNumberInMessage; i++)
                            atmCells.Add(port.GetAtmCell());

                        //Console.WriteLine(DateTime.Now.Millisecond + "  Wysyłanie CableCloudMessage na port " +
                        //                 message.PortNumber + " Liczba ATMCell: " + message.AtmCells.Count
                        //                + " Port: " + port.GetPortNumber());
                        var message = new CableCloudMessage(port.GetPortNumber(), atmCells);
                        SendCableCloudMessage(message);
                        sent = true;
                    }
                if (!sent)
                    Thread.Sleep(10);
            }
        }

        protected override void Recieve(CableCloudMessage cableCloudMessage) {
            ReceiveCableCloudMessage(cableCloudMessage);
        }

        public void ReceiveCableCloudMessage(CableCloudMessage cableCloudMessage) {
            Console.WriteLine("[" + DateTime.Now + "] Message recieved on port: " + cableCloudMessage.PortNumber);

            foreach (var cell in AtmCells(cableCloudMessage))
                CommutationMatrix.AddAtmCellToInputPort(cell, cableCloudMessage.PortNumber);
        }

        private void SendCableCloudMessage(CableCloudMessage cableCloudMessage) {
            Send(cableCloudMessage);
            Console.WriteLine("[" + DateTime.Now + "] Message sent on port: " + cableCloudMessage.PortNumber);
        }
    }
}