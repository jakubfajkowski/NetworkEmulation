using System.Collections.Generic;
using System.Linq;

namespace NetworkUtilities.ControlPlane {
    public class Directory : ControlPlaneElement {
        public static readonly NetworkAddress Address = new NetworkAddress("0");

        private readonly Dictionary<string, NetworkAddress> _clientAdderssDictionary = new Dictionary<string, NetworkAddress>();
        private readonly Dictionary<string, NetworkAddress> _snppDictionary = new Dictionary<string, NetworkAddress>();

        public Directory(NetworkAddress networkAddress) : base(networkAddress) {}

        private void SendDirectoryAddressResponse(SignallingMessage message) {
            var clientNames = (string[]) message.Payload;

            var clientAddressA = _clientAdderssDictionary[clientNames[0]];
            var clientAddressZ = _clientAdderssDictionary[clientNames[1]];

            NetworkAddress[] clientAddress = {clientAddressA, clientAddressZ};

            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectoryAddressResponse;
            directioryResponse.Payload = clientAddress;
            directioryResponse.DestinationAddress = message.SourceAddress;
            SendMessage(directioryResponse);
        }

        private void SendDirectoryNameResponse(SignallingMessage message) {
            var clientAddress = (NetworkAddress[])message.Payload;

            var clientNameA = _clientAdderssDictionary.FirstOrDefault(x => x.Value == clientAddress[0]).Key;
            var clientNameZ = _clientAdderssDictionary.FirstOrDefault(x => x.Value == clientAddress[1]).Key;

            string[] clientName = {clientNameA, clientNameZ};

            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectoryNameResponse;
            directioryResponse.Payload = clientName;
            directioryResponse.DestinationAddress = message.SourceAddress;
            SendMessage(directioryResponse);
        }

        private void SendDirectorySnppResponse(SignallingMessage message) {
            var clientNames = (string[])message.Payload;

            var snmpA = _snppDictionary[clientNames[0]];
            var snmpZ = _snppDictionary[clientNames[1]];

            NetworkAddress[] snpp = { snmpA, snmpZ };

            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectorySnppResponse;
            directioryResponse.Payload = snpp;
            directioryResponse.DestinationAddress = message.SourceAddress;
            SendMessage(directioryResponse);
        }

        public void UpdateDierctory(string clientName, NetworkAddress clientAddress, NetworkAddress snpp) {
            _clientAdderssDictionary.Add(clientName, clientAddress);
            _snppDictionary.Add(clientName, snpp);
        }

        public override void ReceiveMessage(SignallingMessage message) {
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