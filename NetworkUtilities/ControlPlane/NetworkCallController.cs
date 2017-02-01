using System;
using System.Collections.Generic;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class NetworkCallController : ControlPlaneElement {
        private readonly Dictionary<UniqueId, string[]> _nameDictionary =
            new Dictionary<UniqueId, string[]>();

        private readonly Dictionary<UniqueId, NetworkAddress[]> _networkAddressDictionary =
            new Dictionary<UniqueId, NetworkAddress[]>();

        private readonly Dictionary<UniqueId, SubnetworkPointPool[]> _snppDictionary =
            new Dictionary<UniqueId, SubnetworkPointPool[]>();


        public NetworkCallController(NetworkAddress networkAddress) : 
            base(networkAddress, ControlPlaneElementType.NCC) {}

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.CallRequest:
                    HandleCallRequest(message);
                    break;

                case OperationType.PolicyRequest:
                    HandlePolicyRequest(message);
                    break;

                case OperationType.DirectorySnppRequest:
                    HandleDirectorySnppRequest(message);
                    break;

                case OperationType.DirectoryAddressRequest:
                    HandleDirectoryAddressRequest(message);
                    break;

                case OperationType.CallCoordination:
                    HandleCallCoordination(message);
                    break;

                case OperationType.DirectoryNameRequest:
                    HandleDirectoryNameRequest(message);
                    break;

                case OperationType.CallAccept:
                    HandleCallAccept(message);
                    break;

                case OperationType.CallConfirmation:
                    HandleCallConfirmation(message);
                    break;

                case OperationType.ConnectionRequest:
                    HandleConnectionRequest(message);
                    break;

                case OperationType.CallTeardown:
                    //SendCallTeardownResponse(message);
                    break;

                case OperationType.CallTeardownResponse:

                    break;

                case OperationType.CallCoordinationResponse:

                    break;

                case OperationType.CallAcceptResponse:

                    break;

                case OperationType.ConnectionConfirmationToNCC:
                    break;
            }
        }

        private void HandleCallRequest(SignallingMessage message) {
            var clientNames = (string[])message.Payload;

            _nameDictionary.Add(message.SessionId, clientNames);

            SendPolicyRequest(message);
        }

        private void HandlePolicyRequest(SignallingMessage message) {
            if ((bool)message.Payload)
                SendDirectorySnppRequest(message);
        }

        private void HandleDirectorySnppRequest(SignallingMessage message) {
            var snpp = (SubnetworkPointPool[])message.Payload;
            _snppDictionary.Add(message.SessionId, snpp);
            SendDirectoryAddressRequest(message);
        }

        private void HandleDirectoryAddressRequest(SignallingMessage message) {
            var networkAdress = (NetworkAddress[])message.Payload;

            try {
                _networkAddressDictionary.Add(message.SessionId, networkAdress);

                if (networkAdress[0].GetId(0) == networkAdress[1].GetId(0))
                    SendCallAccept(message);
                else
                    SendCallCoordination(message);
            }
            catch (ArgumentOutOfRangeException) {
                SendCallConfirmationToCPCC(message);
            }
        }

        private void HandleCallCoordination(SignallingMessage message) {
            var networkAddress = (NetworkAddress[])message.Payload;

            _networkAddressDictionary.Add(message.SessionId, networkAddress);

            SendDirectoryNameRequest(message);
        }

        private void HandleDirectoryNameRequest(SignallingMessage message) {
            var clientName = (string[])message.Payload;

            _nameDictionary.Add(message.SessionId, clientName);

            SendCallAccept(message);
        }

        private void HandleCallAccept(SignallingMessage message) {
            if (_networkAddressDictionary[message.SessionId][0].DomainId == Address.DomainId)
                SendConnectionRequest(message);
            else
                SendCallConfirmationToNCC(message);
        }

        private void HandleCallConfirmation(SignallingMessage message) {
            SendConnectionRequest(message);
        }

        private void HandleConnectionRequest(SignallingMessage message) {
            SendCallConfirmationToCPCC(message);
        }

        private void SendPolicyRequest(SignallingMessage message) {
            var policyRequest = message;
            policyRequest.Operation = OperationType.PolicyRequest;
            policyRequest.DestinationAddress = NameServer.Address;
            policyRequest.DestinationControlPlaneElement = ControlPlaneElementType.Policy;

            SendMessage(policyRequest);
        }

        private void SendDirectoryAddressRequest(SignallingMessage message) {
            var directioryRequest = message;
            directioryRequest.Operation = OperationType.DirectoryAddressRequest;
            directioryRequest.Payload = _nameDictionary[message.SessionId];
            directioryRequest.DestinationAddress = NameServer.Address;
            directioryRequest.DestinationControlPlaneElement = ControlPlaneElementType.Directory;

            SendMessage(directioryRequest);
        }

        private void SendDirectorySnppRequest(SignallingMessage message) {
            var directioryRequest = message;
            directioryRequest.Operation = OperationType.DirectorySnppRequest;
            directioryRequest.Payload = _nameDictionary[message.SessionId];
            directioryRequest.DestinationAddress = NameServer.Address;
            directioryRequest.DestinationControlPlaneElement = ControlPlaneElementType.Directory;

            SendMessage(directioryRequest);
        }

        private void SendDirectoryNameRequest(SignallingMessage message) {
            var directioryRequest = message;
            directioryRequest.Operation = OperationType.DirectoryNameRequest;
            directioryRequest.Payload = (NetworkAddress[]) message.Payload;
            directioryRequest.DestinationAddress = NameServer.Address;
            directioryRequest.DestinationControlPlaneElement = ControlPlaneElementType.Directory;

            SendMessage(directioryRequest);
        }

        private void SendCallCoordination(SignallingMessage message) {
            var callCoordination = message;
            callCoordination.Operation = OperationType.CallCoordination;
            callCoordination.Payload = (NetworkAddress[]) message.Payload;
            callCoordination.DestinationAddress = _networkAddressDictionary[message.SessionId][1].GetRootFromBeginning(1);
            callCoordination.DestinationControlPlaneElement =
                ControlPlaneElementType.NCC;

            SendMessage(callCoordination);
        }

        private void SendConnectionRequest(SignallingMessage message) {
            var connectionRequest = message;
            var snpp = _snppDictionary[message.SessionId];
            connectionRequest.Operation = OperationType.ConnectionRequest;
            connectionRequest.Payload = snpp;
            connectionRequest.DestinationAddress =
                _networkAddressDictionary[message.SessionId][1].GetRootFromBeginning(1);
            connectionRequest.DestinationControlPlaneElement =
                ControlPlaneElementType.CC;

            SendMessage(connectionRequest);
        }

        private void SendCallAccept(SignallingMessage message) {
            var callAccept = message;
            var clientNames = _nameDictionary[message.SessionId];
            callAccept.Operation = OperationType.CallAccept;
            callAccept.Payload = clientNames;
            callAccept.DestinationAddress = _networkAddressDictionary[message.SessionId][1];
            callAccept.DestinationControlPlaneElement =
                ControlPlaneElementType.CPCC;

            SendMessage(callAccept);
        }

        private void SendCallConfirmationToNCC(SignallingMessage message) {
            var callConfirmation = message;
            callConfirmation.Operation = OperationType.CallConfirmation;
            callConfirmation.DestinationAddress = OtherDomainAddress();
            callConfirmation.DestinationControlPlaneElement =
                ControlPlaneElementType.NCC;

            SendMessage(callConfirmation);
        }

        private void SendCallConfirmationToCPCC(SignallingMessage message) {
            var callConfirmation = message;
            callConfirmation.Operation = OperationType.CallConfirmation;
            callConfirmation.DestinationAddress = _networkAddressDictionary[message.SessionId][0];
            callConfirmation.DestinationControlPlaneElement =
                ControlPlaneElementType.CPCC;

            SendMessage(callConfirmation);
        }

        //private void SendCallCoordinationResponse(SignallingMessage message) {
        //    var callCoordinationResponse = message;
        //    callCoordinationResponse.Operation = OperationType.CallCoordinationResponse;
        //    callCoordinationResponse.Payload = true;
        //    callCoordinationResponse.DestinationAddress = message.SourceAddress;

        //    SendMessage(callCoordinationResponse);
        //}

        //private void SendCallRequestResponse(SignallingMessage message) {
        //    var callRequestResponse = message;
        //    callRequestResponse.Operation = OperationType.CallRequestResponse;
        //    callRequestResponse.Payload = true;
        //    callRequestResponse.DestinationAddress = message.SourceAddress;

        //    SendMessage(callRequestResponse);
        //}

        //private void SendCallTeardownResponse(SignallingMessage message) {
        //    var callTeardownResponse = message;
        //    callTeardownResponse.Operation = OperationType.CallTeardownResponse;
        //    callTeardownResponse.Payload = true;
        //    callTeardownResponse.DestinationAddress = message.SourceAddress;

        //    SendMessage(callTeardownResponse);
        //}

        public NetworkAddress OtherDomainAddress() {
            if (Address.DomainId == 1) return new NetworkAddress(2);
            if (Address.DomainId == 2) return new NetworkAddress(1);
            return null;
        }
    }
}