﻿using System;
using System.Collections.Generic;
using System.Threading;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.DataPlane;
using NetworkUtilities.ManagementPlane;

namespace NetworkUtilities.Network.NetworkNode {
    public class NetworkNode : Node.Node {
        private readonly ConnectionController _connectionController;
        private readonly LinkResourceManager _linkResourceManager;

        private Thread _networkNodeThread;

        private bool _timeToQuit;

        public CommutationMatrix CommutationMatrix;
        public NetworkNodeAgent NetworkNodeAgent;

        public NetworkNode(NetworkNodeModel parameters)
            : base(
                parameters.NetworkAddress, parameters.IpAddress, parameters.CableCloudListeningPort,
                parameters.SignallingCloudListeningPort) {
            CableCloudMessage.MaxAtmCellsNumber = parameters.MaxAtmCellsNumberInCableCloudMessage;

            CommutationMatrix = new CommutationMatrix(new CommutationTable(), parameters.NumberOfPorts);
            CommutationMatrix.UpdateState += (sender, state) => OnUpdateState(state);

            _connectionController = new ConnectionController(parameters.NetworkAddress);
            _connectionController.UpdateState += (sender, state) => OnUpdateState(state);
            _connectionController.MessageToSend += (sender, message) => Send(message);


            _linkResourceManager = new LinkResourceManager(parameters.NetworkAddress);
            _linkResourceManager.UpdateState += (sender, state) => OnUpdateState(state);
            _linkResourceManager.MessageToSend += (sender, message) => Send(message);
            _linkResourceManager.CommutationCommand += ConnectionControllerOnCommutationCommand;

            NetworkNodeAgent = new NetworkNodeAgent(parameters.NetworkAddress, parameters.IpAddress,
                parameters.NetworkManagmentSystemListeningPort);
            NetworkNodeAgent.UpdateState += (sender, state) => OnUpdateState(state);
            NetworkNodeAgent.ConfigurationReceived += NetworkNodeAgentOnConfigurationReceived;

            StartThread();
        }

        private void NetworkNodeAgentOnConfigurationReceived(object sender, Link link) {
            var configurationMessage = new SignallingMessage {
                Payload = link,
                Operation = OperationType.Configuration,
                DestinationAddress = NetworkAddress,
                DestinationControlPlaneElement = ControlPlaneElementType.LRM
            };

            Receive(configurationMessage);
        }

        // Czas po jakim komórki ATM zostaną spakowane w CCM
        public static int MinLastAddTime { private get; set; } = 100;

        private void ConnectionControllerOnCommutationCommand(object sender, CommutationHandlerArgs args) {
            CommutationMatrix._commutationTable.AddConnection(args.CommutationTableRow);
        }

        public override void Initialize() {
            base.Initialize();
            NetworkNodeAgent.Initialize();
        }

        public void StartThread() {
            //_timeToQuit = false;
            _networkNodeThread = new Thread(RunThread);
            _networkNodeThread.Start();
            CommutationMatrix.StartThread();
        }

        public void Shutdown() {
            //_timeToQuit = true;
            CommutationMatrix.Shutdown();
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
            OnUpdateState("[RECEIVED] Port: " + cableCloudMessage.PortNumber + ", " + cableCloudMessage.ExtractAtmCells().Count +
                          " cells");

            /* foreach (var cell in ExtractAtmCells(cableCloudMessage))
                 CommutationMatrix.AddAtmCellToInputPort(cell, cableCloudMessage.PortNumber);
                 */
            var message = CommutationMatrix.CommuteAllCells(cableCloudMessage.ExtractAtmCells(),
                cableCloudMessage.PortNumber);

            if (message != null) {
                SendCableCloudMessage(message);
            }
            else {
                OnUpdateState("[UNKNOWN_DESTINATION]");
            }
        }

        private void SendCableCloudMessage(CableCloudMessage cableCloudMessage) {
            Send(cableCloudMessage);
            OnUpdateState("[" + DateTime.Now + "] Message sent on port: " + cableCloudMessage.PortNumber);
            OnUpdateState("[" + DateTime.Now + "] Sent " + cableCloudMessage.ExtractAtmCells().Count + " atmcells");
        }

        protected override void Receive(SignallingMessage signallingMessage) {
            switch (signallingMessage.DestinationControlPlaneElement) {
                case ControlPlaneElementType.CC:
                    _connectionController.ReceiveMessage(signallingMessage);
                    break;

                case ControlPlaneElementType.LRM:
                    _linkResourceManager.ReceiveMessage(signallingMessage);
                    break;
            }
        }
    }
}