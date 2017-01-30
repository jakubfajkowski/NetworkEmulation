﻿using System;
using System.Collections.Generic;
using System.Threading;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Element;
using NetworkUtilities.Network;

namespace NetworkNode {
    public class NetworkNode : Node {
        private ConnectionController _connectionController;
        private List<LinkResourceManager> _linkResourceManagers;

        private Thread _networkNodeThread;

        private bool _timeToQuit;

        public CommutationMatrix CommutationMatrix;
        public NetworkNodeAgent NetworkNodeAgent;

        public NetworkNode(NetworkNodeModel parameters)
            : base(
                parameters.NetworkAddress, parameters.NetworkAddress.GetParentsAddress(), parameters.IpAddress, parameters.CableCloudListeningPort,
                parameters.PathComputationServerListeningPort) {

            CableCloudMessage.MaxAtmCellsNumber = parameters.MaxAtmCellsNumberInCableCloudMessage;
            NetworkNodeAgent.NmsPort = parameters.NetworkManagmentSystemListeningPort;
            NetworkNodeAgent = new NetworkNodeAgent(parameters.NetworkManagmentSystemDataPort, this);
            OnUpdateState("Network Node \nNMS port: " + parameters.NetworkManagmentSystemDataPort);
            CommutationMatrix = new CommutationMatrix(NetworkNodeAgent.GetCommutationTable(), parameters.NumberOfPorts);
            CommutationMatrix.UpdateState += (sender, state) => OnUpdateState(state);
            NetworkNodeAgent.SetCommutationMatrix(CommutationMatrix);

            _connectionController = new ConnectionController(parameters.NetworkAddress);
            _connectionController.UpdateState += (sender, state) => OnUpdateState(state);
            _connectionController.MessageToSend += (sender, message) => Send(message);


            var _linkResourceManager = new LinkResourceManager(parameters.NetworkAddress, null, 0, 0);
            _linkResourceManager.UpdateState += (sender, state) => OnUpdateState(state);
            _linkResourceManager.MessageToSend += (sender, message) => Send(message);

            StartThread();
        }

        // Czas po jakim komórki ATM zostaną spakowane w CCM
        public static int MinLastAddTime { private get; set; } = 100;

        public void StartThread() {
            //_timeToQuit = false;
            _networkNodeThread = new Thread(RunThread);
            _networkNodeThread.Start();
            CommutationMatrix.StartThread();
            NetworkNodeAgent.StartThread();
        }

        public void Shutdown() {
            //_timeToQuit = true;
            CommutationMatrix.Shutdown();
            NetworkNodeAgent.Shutdown();
        }

        /* Wątek pobierający komórki ATM z portów wyjściowych pola komutacyjnego i wysyłający je do chmury kablowej */

        private void RunThread() {
            while (!_timeToQuit) {
                var sent = false;
                foreach (var port in CommutationMatrix.OutputPorts)
                    if (port.GetAtmCellNumber() != 0 &&
                        (DateTime.Now - port.GetLastAddTime()).TotalMilliseconds > MinLastAddTime ||
                        port.GetAtmCellNumber() > CableCloudMessage.MaxAtmCellsNumber) {
                        // OnUpdateState("Różnica czasu: " + (DateTime.Now - port.GetLastAddTime()).TotalMilliseconds + "   if max: " + (port.GetAtmCellNumber() > CableCloudMessage.MaxAtmCellsNumber));
                        var atmCells = new List<AtmCell>();

                        var atmCellNumberInPort = port.GetAtmCellNumber();
                        int atmCellNumberInMessage;
                        if (atmCellNumberInPort > CableCloudMessage.MaxAtmCellsNumber)
                            atmCellNumberInMessage = CableCloudMessage.MaxAtmCellsNumber;
                        else
                            atmCellNumberInMessage = atmCellNumberInPort;

                        for (var i = 0; i < atmCellNumberInMessage; i++)
                            atmCells.Add(port.GetAtmCell());

                        //OnUpdateState(DateTime.Now.Millisecond + "  Wysyłanie CableCloudMessage na port " +
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

        protected override void Receive(CableCloudMessage cableCloudMessage) {
            ReceiveCableCloudMessage(cableCloudMessage);
        }

        public void ReceiveCableCloudMessage(CableCloudMessage cableCloudMessage) {
            OnUpdateState("[" + DateTime.Now + "] Message received on port: " + cableCloudMessage.PortNumber);
            OnUpdateState("[" + DateTime.Now + "] Received " + cableCloudMessage.ExtractAtmCells().Count +
                              " atmcells");

            /* foreach (var cell in ExtractAtmCells(cableCloudMessage))
                 CommutationMatrix.AddAtmCellToInputPort(cell, cableCloudMessage.PortNumber);
                 */

            SendCableCloudMessage(CommutationMatrix.CommuteAllCells(cableCloudMessage.ExtractAtmCells(),
                cableCloudMessage.PortNumber));
        }

        private void SendCableCloudMessage(CableCloudMessage cableCloudMessage) {
            Send(cableCloudMessage);
            OnUpdateState("[" + DateTime.Now + "] Message sent on port: " + cableCloudMessage.PortNumber);
            OnUpdateState("[" + DateTime.Now + "] Sent " + cableCloudMessage.ExtractAtmCells().Count + " atmcells");
        }

        protected override void Receive(SignallingMessage signallingMessage) {
            //TODO Signalling message handling implementation!
            throw new NotImplementedException();
        }
    }
}