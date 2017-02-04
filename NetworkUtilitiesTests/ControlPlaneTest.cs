using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class ControlPlaneTest {
        private readonly Dictionary<Row, ControlPlaneElement> _controlPlaneElements =
            new Dictionary<Row, ControlPlaneElement>();

        [TestMethod]
        public void TestMessageTransfer() {
            var directory = new Directory(NameServer.Address);
            _controlPlaneElements.Add(new Row(NameServer.Address, ControlPlaneElementType.Directory), directory);

            var policy = new Policy(NameServer.Address);
            _controlPlaneElements.Add(new Row(NameServer.Address, ControlPlaneElementType.Policy), policy);

            var ncc1Address = new NetworkAddress("1");
            var ncc1 = new NetworkCallController(ncc1Address);
            _controlPlaneElements.Add(new Row(ncc1Address, ControlPlaneElementType.NCC), ncc1);

            var ncc2Address = new NetworkAddress("2");
            var ncc2 = new NetworkCallController(ncc2Address);
            _controlPlaneElements.Add(new Row(ncc2Address, ControlPlaneElementType.NCC), ncc2);

            var cpccAAddress = new NetworkAddress("1.1.1");
            var cpccA = new CallingPartyCallController(cpccAAddress);
            _controlPlaneElements.Add(new Row(cpccAAddress, ControlPlaneElementType.CPCC), cpccA);
            directory.UpdateDirectory("A", cpccAAddress, new SubnetworkPointPool(cpccAAddress.Append(1)));

            var cpccBAddress = new NetworkAddress("2.1.2");
            var cpccB = new CallingPartyCallController(cpccBAddress);
            _controlPlaneElements.Add(new Row(cpccBAddress, ControlPlaneElementType.CPCC), cpccB);
            directory.UpdateDirectory("B", cpccBAddress, new SubnetworkPointPool(cpccBAddress.Append(1)));

            var cc1Address = new NetworkAddress("1");
            var cc1 = new ConnectionController(cc1Address);
            _controlPlaneElements.Add(new Row(cc1Address, ControlPlaneElementType.CC), cc1);

            var cc2Address = new NetworkAddress("2");
            var cc2 = new ConnectionController(cc2Address);
            _controlPlaneElements.Add(new Row(cc2Address, ControlPlaneElementType.CC), cc2);

            var rc2Address = new NetworkAddress("2");
            var rc2 = new RoutingController(rc2Address);
            _controlPlaneElements.Add(new Row(rc2Address, ControlPlaneElementType.RC), rc2);

            directory.MessageToSend += PassMessage;
            policy.MessageToSend += PassMessage;
            ncc1.MessageToSend += PassMessage;
            ncc2.MessageToSend += PassMessage;
            cpccA.MessageToSend += PassMessage;
            cpccB.MessageToSend += PassMessage;
            cc1.MessageToSend += PassMessage;
            cc2.MessageToSend += PassMessage;
            rc2.MessageToSend += PassMessage;

            directory.UpdateState += UpdateState;
            policy.UpdateState += UpdateState;
            ncc1.UpdateState += UpdateState;
            ncc2.UpdateState += UpdateState;
            cpccA.UpdateState += UpdateState;
            cpccB.UpdateState += UpdateState;
            cc1.UpdateState += UpdateState;
            cc2.UpdateState += UpdateState;
            rc2.UpdateState += UpdateState;

            cpccA.SendCallRequest("A", "B", 20);

            Thread.Sleep(10000);
        }

        [TestMethod]
        public void TestLRM() {
            var client1Address = new NetworkAddress("1.1.1");
            var client2Address = new NetworkAddress("1.2.1");

            var node1Address = new NetworkAddress("1.1");
            var node2Address = new NetworkAddress("1.2");

            var node1Client1SnppOut = new SubnetworkPointPool(client1Address.Append(1));
            var node1Client1SnppIn = new SubnetworkPointPool(node1Address.Append(1));
            var node1Node2SnppOut = new SubnetworkPointPool(node2Address.Append(2));
            var node1Node2SnppIn = new SubnetworkPointPool(node1Address.Append(2));

            var client1Node1Link = new Link(node1Client1SnppIn, node1Client1SnppOut, 10, true);
            var node1Node2Link = new Link(node1Node2SnppIn, node1Node2SnppOut, 10, false);

            var cc1Address = node1Address;
            var cc1 = new ConnectionController(cc1Address);
            _controlPlaneElements.Add(new Row(cc1Address, ControlPlaneElementType.CC), cc1);

            var lrm1Address = node1Address;
            var lrm1 = new LinkResourceManager(lrm1Address);
            _controlPlaneElements.Add(new Row(lrm1Address, ControlPlaneElementType.LRM), lrm1);

            var lrm2Address = node2Address;
            var lrm2 = new LinkResourceManager(lrm2Address);
            _controlPlaneElements.Add(new Row(lrm2Address, ControlPlaneElementType.LRM), lrm2);

            cc1.MessageToSend += PassMessage;
            lrm1.MessageToSend += PassMessage;
            lrm2.MessageToSend += PassMessage;

            cc1.UpdateState += UpdateState;
            lrm1.UpdateState += UpdateState;
            lrm2.UpdateState += UpdateState;

            lrm1.ReceiveMessage(new SignallingMessage {
                Operation = OperationType.Configuration,
                Payload = client1Node1Link,
                DestinationAddress = node1Address,
                DestinationControlPlaneElement = ControlPlaneElementType.LRM
            });

            lrm1.ReceiveMessage(new SignallingMessage {
                Operation = OperationType.Configuration,
                Payload = client1Node1Link.Reverse(),
                DestinationAddress = node1Address,
                DestinationControlPlaneElement = ControlPlaneElementType.LRM
            });

            lrm1.ReceiveMessage(new SignallingMessage {
                Operation = OperationType.Configuration,
                Payload = node1Node2Link,
                DestinationAddress = node1Address,
                DestinationControlPlaneElement = ControlPlaneElementType.LRM
            });

            lrm1.ReceiveMessage(new SignallingMessage {
                Operation = OperationType.Configuration,
                Payload = node1Node2Link.Reverse(),
                DestinationAddress = node1Address,
                DestinationControlPlaneElement = ControlPlaneElementType.LRM
            });

            lrm2.ReceiveMessage(new SignallingMessage {
                Operation = OperationType.Configuration,
                Payload = node1Node2Link,
                DestinationAddress = node2Address,
                DestinationControlPlaneElement = ControlPlaneElementType.LRM
            });

            lrm2.ReceiveMessage(new SignallingMessage {
                Operation = OperationType.Configuration,
                Payload = node1Node2Link.Reverse(),
                DestinationAddress = node2Address,
                DestinationControlPlaneElement = ControlPlaneElementType.LRM
            });

            cc1.ReceiveMessage(new SignallingMessage {
                DemandedCapacity = 5,
                DestinationAddress = cc1Address,
                DestinationControlPlaneElement = ControlPlaneElementType.CC,
                DestinationClientAddress = client2Address,
                Operation = OperationType.RouteTableQuery,
                Payload = new Queue<SubnetworkPointPool>(new[] {
                    node1Client1SnppIn,
                    node1Node2SnppOut
                }),
                SourceAddress = new NetworkAddress("0"),
                SourceClientAddress = client1Address,
                SourceControlPlaneElement = ControlPlaneElementType.RC
            });
        }

        private void UpdateState(object sender, string state) {
            Console.WriteLine(state);
        }

        private void PassMessage(object sender, SignallingMessage message) {
            try {
                var destination =
                    _controlPlaneElements[new Row(message.DestinationAddress, message.DestinationControlPlaneElement)];
                Thread.Sleep(10);
                destination.ReceiveMessage(message);
            }
            catch (KeyNotFoundException) {
                Console.WriteLine($"[ADDRESS_NOT_FOUND] {message.DestinationAddress}.{message.DestinationControlPlaneElement}");
            }
        }
    }

    internal class Row {
        public Row(NetworkAddress networkAddress, ControlPlaneElementType controlPlaneElement) {
            NetworkAddress = networkAddress;
            ControlPlaneElement = controlPlaneElement;
        }

        public NetworkAddress NetworkAddress { get; }
        public ControlPlaneElementType ControlPlaneElement { get; }

        protected bool Equals(Row other) {
            return Equals(NetworkAddress, other.NetworkAddress) && ControlPlaneElement == other.ControlPlaneElement;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Row)) return false;
            return Equals((Row) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((NetworkAddress != null ? NetworkAddress.GetHashCode() : 0) * 397) ^ (int) ControlPlaneElement;
            }
        }
    }
}