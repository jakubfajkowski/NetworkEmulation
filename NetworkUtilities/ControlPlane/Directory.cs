using System.Collections.Generic;
using System.Linq;

namespace NetworkUtilities.ControlPlane {
    public class Directory : ControlPlaneElement {
        private static readonly Dictionary<string, NetworkAddress> _clientAdderssDictionary =
            new Dictionary<string, NetworkAddress>();

        private static readonly Dictionary<string, SubnetworkPointPool> _snppDictionary =
            new Dictionary<string, SubnetworkPointPool>();

        public Directory(NetworkAddress networkAddress) : base(networkAddress) {
        }

        private void SendDirectoryAddressResponse(SignallingMessage message) {
            var clientNames = (string[]) message.Payload;

            var clientAddressA = _clientAdderssDictionary[clientNames[0]];
            var clientAddressZ = _clientAdderssDictionary[clientNames[1]];

            NetworkAddress[] clientAddress = {clientAddressA, clientAddressZ};

            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectoryAddressResponse;
            directioryResponse.Payload = clientAddress;
            directioryResponse.DestinationAddress = message.SourceAddress;
            directioryResponse.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.NetworkCallController;
            SendMessage(directioryResponse);
        }

        private void SendDirectoryNameResponse(SignallingMessage message) {
            var clientAddress = (NetworkAddress[]) message.Payload;

            var clientNameA = _clientAdderssDictionary.FirstOrDefault(x => x.Value == clientAddress[0]).Key;
            var clientNameZ = _clientAdderssDictionary.FirstOrDefault(x => x.Value == clientAddress[1]).Key;

            string[] clientName = {clientNameA, clientNameZ};

            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectoryNameResponse;
            directioryResponse.Payload = clientName;
            directioryResponse.DestinationAddress = message.SourceAddress;
            directioryResponse.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.NetworkCallController;
            SendMessage(directioryResponse);
        }

        private void SendDirectorySnppResponse(SignallingMessage message) {
            var clientNames = (string[]) message.Payload;

            var snmpA = _snppDictionary[clientNames[0]];
            var snmpZ = _snppDictionary[clientNames[1]];

            SubnetworkPointPool[] snpp = {snmpA, snmpZ};

            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectorySnppResponse;
            directioryResponse.Payload = snpp;
            directioryResponse.DestinationAddress = message.SourceAddress;
            directioryResponse.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.NetworkCallController;
            SendMessage(directioryResponse);
        }

        public static void UpdateDirectory(string clientName, SubnetworkPointPool snpp) {
            _clientAdderssDictionary.Add(clientName, snpp.NetworkAddress.GetParentsAddress());
            _snppDictionary.Add(clientName, snpp);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case SignallingMessageOperation.DirectoryAddressRequest:
                    SendDirectoryAddressResponse(message);
                    break;
                case SignallingMessageOperation.DirectoryNameRequest:
                    SendDirectoryNameResponse(message);
                    break;
                case SignallingMessageOperation.DirectorySnppRequest:
                    SendDirectorySnppResponse(message);
                    break;
            }
        }
    }
}