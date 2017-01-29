using System.Collections.Generic;
using NetworkUtilities.GraphAlgorithm;

namespace NetworkUtilities.ControlPlane {
    public class NetworkCallController : ControlPlaneElement {
        private readonly Dictionary<UniqueId, int> _capacityDictionary =
            new Dictionary<UniqueId, int>();

        private readonly Dictionary<UniqueId, string[]> _nameDictionary =
            new Dictionary<UniqueId, string[]>();

        private readonly Dictionary<UniqueId, NetworkAddress[]> _networkAddressDictionary =
            new Dictionary<UniqueId, NetworkAddress[]>();

        private readonly Dictionary<UniqueId, SubnetworkPointPool[]> _snppDictionary =
            new Dictionary<UniqueId, SubnetworkPointPool[]>();


        public NetworkCallController(NetworkAddress networkAddress) : base(networkAddress) {
        }

        private void SendPolicyRequest(SignallingMessage message) {
            var policyRequest = message;
            policyRequest.Operation = SignallingMessageOperation.PolicyRequest;
            policyRequest.DestinationAddress = NameServer.Address;
            policyRequest.DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.Policy;
            SendMessage(policyRequest);
        }

        private void SendDirectoryAddressRequest(SignallingMessage message) {
            var directioryRequest = message;
            directioryRequest.Operation = SignallingMessageOperation.DirectoryAddressRequest;
            directioryRequest.Payload = _nameDictionary[message.SessionId];
            directioryRequest.DestinationAddress = NameServer.Address;
            directioryRequest.DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.Directory;
            SendMessage(directioryRequest);
        }

        private void SendDirectorySnppRequest(SignallingMessage message) {
            var directioryRequest = message;
            directioryRequest.Operation = SignallingMessageOperation.DirectorySnppRequest;
            directioryRequest.Payload = _nameDictionary[message.SessionId];
            directioryRequest.DestinationAddress = NameServer.Address;
            directioryRequest.DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.Directory;
            SendMessage(directioryRequest);
        }

        private void SendDirectoryNameRequest(SignallingMessage message) {
            var directioryRequest = message;
            directioryRequest.Operation = SignallingMessageOperation.DirectoryNameRequest;
            directioryRequest.Payload = (NetworkAddress[]) message.Payload;
            directioryRequest.DestinationAddress = NameServer.Address;
            directioryRequest.DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.Directory;
            SendMessage(directioryRequest);
        }

        private void SendCallCoordination(SignallingMessage message) {
            var callCoordination = message;
            callCoordination.Operation = SignallingMessageOperation.CallCoordination;
            callCoordination.Payload = (NetworkAddress[]) message.Payload;
            callCoordination.DestinationAddress = _networkAddressDictionary[message.SessionId][1].GetRootFromBeginning(1);
            callCoordination.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.NetworkCallController;
            SendMessage(callCoordination);
        }

        private void SendConnectionRequest(SignallingMessage message) {
            var connectionRequest = message;
            var snpp = _snppDictionary[message.SessionId];
            var capacity = _capacityDictionary[message.SessionId];

            object[] connectionRequestMessage = {snpp, capacity};
            connectionRequest.Operation = SignallingMessageOperation.ConnectionRequest;
            connectionRequest.Payload = connectionRequestMessage;
            connectionRequest.DestinationAddress =
                _networkAddressDictionary[message.SessionId][1].GetRootFromBeginning(1);
            //connectionRequest.DestinationAddress = new NetworkAddress("0.1");
            connectionRequest.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.ConnectionController;
            SendMessage(connectionRequest);
        }

        private void SendCallAccept(SignallingMessage message) {
            var callAccept = message;
            var clientNames = _nameDictionary[message.SessionId];
            callAccept.Operation = SignallingMessageOperation.CallAccept;
            callAccept.Payload = clientNames;
            callAccept.DestinationAddress = _networkAddressDictionary[message.SessionId][1];
            callAccept.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.CallingPartyCallController;
            SendMessage(callAccept);
        }

        private void SendCallConfirmationToNCC(SignallingMessage message) {
            var callConfirmation = message;
            callConfirmation.Operation = SignallingMessageOperation.CallConfirmationFromNCC;
            callConfirmation.DestinationAddress = _networkAddressDictionary[message.SessionId][0].GetRootFromBeginning(1);
            callConfirmation.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.NetworkCallController;
            SendMessage(callConfirmation);
        }

        private void SendCallConfirmationToCPCC(SignallingMessage message) {
            var callConfirmation = message;
            callConfirmation.Operation = SignallingMessageOperation.CallConfirmation;
            callConfirmation.DestinationAddress = _networkAddressDictionary[message.SessionId][0];
            callConfirmation.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.CallingPartyCallController;
            SendMessage(callConfirmation);
        }

        private void SendCallCoordinationResponse(SignallingMessage message) {
            var callCoordinationResponse = message;
            callCoordinationResponse.Operation = SignallingMessageOperation.CallCoordinationResponse;
            callCoordinationResponse.Payload = true;
            callCoordinationResponse.DestinationAddress = message.SourceAddress;
            SendMessage(callCoordinationResponse);
        }

        private void SendCallRequestResponse(SignallingMessage message) {
            var callRequestResponse = message;
            callRequestResponse.Operation = SignallingMessageOperation.CallRequestResponse;
            callRequestResponse.Payload = true;
            callRequestResponse.DestinationAddress = message.SourceAddress;
            SendMessage(callRequestResponse);
        }

        private void SendCallTeardownResponse(SignallingMessage message) {
            var callTeardownResponse = message;
            callTeardownResponse.Operation = SignallingMessageOperation.CallTeardownResponse;
            callTeardownResponse.Payload = true;
            callTeardownResponse.DestinationAddress = message.SourceAddress;
            SendMessage(callTeardownResponse);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);
            switch (message.Operation) {
                case SignallingMessageOperation.PolicyResponse:
                    if ((bool)message.Payload) {
                        SendDirectorySnppRequest(message);
                    }
                    break;
                case SignallingMessageOperation.CallRequest:
                    var callRequestMessage = (object[]) message.Payload;
                    var clientNames = (string[]) callRequestMessage[0];
                    var capacity = (int) callRequestMessage[1];

                    _capacityDictionary.Add(message.SessionId, capacity);
                    _nameDictionary.Add(message.SessionId, clientNames);

                    
                    SendPolicyRequest(message);
                    break;
                case SignallingMessageOperation.CallTeardown:
                    SendCallTeardownResponse(message);
                    break;
                case SignallingMessageOperation.CallCoordination:
                    var networkAddress = (NetworkAddress[]) message.Payload;
                    _networkAddressDictionary.Add(message.SessionId, networkAddress);
                    SendDirectoryNameRequest(message);
                    //SendCallCoordinationResponse(message);
                    break;
                case SignallingMessageOperation.CallTeardownResponse:

                    break;
                case SignallingMessageOperation.DirectorySnppResponse:
                    var snpp = (SubnetworkPointPool[]) message.Payload;
                    _snppDictionary.Add(message.SessionId, snpp);
                    SendDirectoryAddressRequest(message);
                    break;
                case SignallingMessageOperation.DirectoryAddressResponse:
                    var networkAdress = (NetworkAddress[]) message.Payload;
                    _networkAddressDictionary.Add(message.SessionId, networkAdress);

                    if (networkAdress[0].GetId(0) == networkAdress[1].GetId(0)) SendCallAccept(message);
                    else SendCallCoordination(message);
                    break;
                case SignallingMessageOperation.DirectoryNameResponse:
                    var clientName = (string[]) message.Payload;
                    _nameDictionary.Add(message.SessionId, clientName);
                    SendCallAccept(message);
                    break;
                case SignallingMessageOperation.CallCoordinationResponse:

                    break;
                case SignallingMessageOperation.CallAcceptResponse:

                    break;
                case SignallingMessageOperation.ConnectionRequestResponse:

                    break;
                case SignallingMessageOperation.CallConfirmation:
                    var address = _networkAddressDictionary[message.SessionId];
                    if (address[0].GetId(0) == address[1].GetId(0)) SendConnectionRequest(message);
                    else SendCallConfirmationToNCC(message);
                    break;
                case SignallingMessageOperation.CallConfirmationFromNCC:
                    SendConnectionRequest(message);
                    break;

                case SignallingMessageOperation.ConnectionConfirmation:
                    SendCallConfirmationToCPCC(message);
                    break;
            }
        }
    }
}