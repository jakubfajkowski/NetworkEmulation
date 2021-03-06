﻿using System.Collections.Generic;
using System.Windows.Forms;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.DataPlane;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.Network.ClientNode {
    public class ClientNode : Node.Node {
        private readonly CallingPartyCallController _callingPartyCallController;
        public List<ClientTableRow> ClientTableList = new List<ClientTableRow>();
        private bool callConfirmation;

        public ClientNode(ClientNodeModel parameters)
            : base(
                parameters.NetworkAddress, parameters.IpAddress, parameters.CableCloudListeningPort,
                parameters.SignallingCloudListeningPort) {
            CableCloudMessage.MaxAtmCellsNumber = parameters.MaxAtmCellsNumberInCableCloudMessage;
            ClientName = parameters.ClientName;

            _callingPartyCallController = new CallingPartyCallController(parameters.NetworkAddress);
            _callingPartyCallController.UpdateState += (sender, state) => OnUpdateState(state);
            _callingPartyCallController.MessageToSend += (sender, message) => Send(message);
        }

        public string ClientName { get; }
        public event MessageHandler OnMessageReceived;
        public event MessageHandler OnClientTableRowAdded;
        public event MessageHandler OnClientTableRowDeleted;

        private void MessageReceived(string message) {
            OnMessageReceived?.Invoke(this, message);
        }

        private void ClientTableRowAdded(string clientName) {
            OnClientTableRowAdded?.Invoke(this, clientName);
        }

        private void ClientTableRowDeleted(string clientName) {
            OnClientTableRowDeleted?.Invoke(this, clientName);
        }

        private void AddClient(ClientTableRow clientTableRow) {
            ClientTableList.Add(clientTableRow);
            ClientTableRowAdded(clientTableRow.ClientName);
        }

        private void DeleteClient(ClientTableRow clientTableRow) {
            ClientTableList.Remove(clientTableRow);
            ClientTableRowDeleted(clientTableRow.ClientName);
        }

        public void Connect(string receiverId, int demandedCapacity) {
            _callingPartyCallController.SendCallRequest(ClientName, receiverId, demandedCapacity);
        }

        public void SendMessage(string message, string receiverId) {
            var clientTableRow = SearchUsingClientName(receiverId);

            if (clientTableRow != null) SendCableCloudMessage(message, clientTableRow);
            else OnUpdateState("Specified client not found");
        }

        public void Disconnect(string receiverId) {
            _callingPartyCallController.SendCallTeardown(ClientName, receiverId);
            DeleteClient(SearchUsingClientName(receiverId));
        }

        private ClientTableRow SearchUsingClientName(string clientName) {
            foreach (var clientTableRow in ClientTableList)
                if (clientTableRow.ClientName.Equals(clientName)) return clientTableRow;
            return null;
        }

        private void SendCableCloudMessage(string message, ClientTableRow clientTableRow) {
            var vci = clientTableRow.Vci;
            var vpi = clientTableRow.Vpi;
            var portNumber = clientTableRow.PortNumber;

            var cableCloudMessages = Generate(portNumber, vpi, vci, message);

            foreach (var cableCloudMessage in cableCloudMessages) {
                Send(cableCloudMessage);
                OnUpdateState("Sent: " + cableCloudMessage.ExtractAtmCells().Count + " ATMCells.");
            }
        }

        public static List<CableCloudMessage> Generate(int portNumber, int vpi, int vci, string message) {
            var atmCells = AtmCell.Generate(vpi, vci, message);
            var cableCloudMessages = new List<CableCloudMessage>();

            while (atmCells.Count >= CableCloudMessage.MaxAtmCellsNumber) {
                var atmCellsPart = atmCells.GetRange(0, CableCloudMessage.MaxAtmCellsNumber);
                atmCells.RemoveRange(0, CableCloudMessage.MaxAtmCellsNumber);
                cableCloudMessages.Add(new CableCloudMessage(portNumber, atmCellsPart));
            }
            cableCloudMessages.Add(new CableCloudMessage(portNumber, atmCells));

            return cableCloudMessages;
        }

        protected override void Receive(CableCloudMessage cableCloudMessage) {
            MessageReceived(cableCloudMessage.ToString());
            OnUpdateState("Received: " + cableCloudMessage.ExtractAtmCells().Count + " ATMCells.");
        }

        protected override void Receive(SignallingMessage message) {
            _callingPartyCallController.ReceiveMessage(message);

            if (message.Operation == OperationType.CallRequestResponse &&
                message.DestinationControlPlaneElement == ControlPlaneElementType.CPCC) {
                var snps = message.Payload as SubnetworkPoint;
                if (snps != null) {
                    AddClient(new ClientTableRow(message.SourceClientName, 1, snps.Vpi, snps.Vci));
                    OnUpdateState(snps.ToString());
                }
                else {
                    OnUpdateState("Connection request rejected");
                    ClientTableRowAdded(null);
                }
             }
            else if (message.Operation == OperationType.CallAccept) {
                var result = MessageBox.Show("Connection incoming from " + ((string[]) message.Payload)[0], "Connection", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                if (result == DialogResult.No) {
                    callConfirmation = false;
                }
                else if(result == DialogResult.Yes) {
                    callConfirmation = true;
                }
                
                _callingPartyCallController.SendCallAccept(message, callConfirmation);
            }
        }
    }
}