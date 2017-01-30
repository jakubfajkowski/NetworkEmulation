using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Utilities;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class ControlPlaneTest {

        private readonly Dictionary<Row, ControlPlaneElement> _controlPlaneElements =
            new Dictionary<Row, ControlPlaneElement>();

        [TestMethod]
        public void TestMessageTransfer() {
                var directory = new Directory(NameServer.Address);
                _controlPlaneElements.Add(new Row(NameServer.Address, SignallingMessageDestinationControlPlaneElement.Directory), directory);

                var policy = new Policy(NameServer.Address);
                _controlPlaneElements.Add(new Row(NameServer.Address, SignallingMessageDestinationControlPlaneElement.Policy), policy);

                var ncc1Address = new NetworkAddress("1");
                var ncc1 = new NetworkCallController(ncc1Address);
                _controlPlaneElements.Add(new Row(ncc1Address, SignallingMessageDestinationControlPlaneElement.NetworkCallController), ncc1);

                var ncc2Address = new NetworkAddress("2");
                var ncc2 = new NetworkCallController(ncc2Address);
                _controlPlaneElements.Add(new Row(ncc2Address, SignallingMessageDestinationControlPlaneElement.NetworkCallController), ncc2);

                var cpccAAddress = new NetworkAddress("1.1");
                var cpccA = new CallingPartyCallController(cpccAAddress);
                _controlPlaneElements.Add(new Row(cpccAAddress, SignallingMessageDestinationControlPlaneElement.CallingPartyCallController), cpccA);

                var cpccBAddress = new NetworkAddress("2.2");
                var cpccB = new CallingPartyCallController(cpccBAddress);
                _controlPlaneElements.Add(new Row(cpccBAddress, SignallingMessageDestinationControlPlaneElement.CallingPartyCallController), cpccB);

                var cc1Address = new NetworkAddress("1");
                var cc1 = new ConnectionController(cc1Address);
                _controlPlaneElements.Add(new Row(cc1Address, SignallingMessageDestinationControlPlaneElement.ConnectionController), cc1);

                var cc2Address = new NetworkAddress("2");
                var cc2 = new ConnectionController(cc2Address);
                _controlPlaneElements.Add(new Row(cc2Address, SignallingMessageDestinationControlPlaneElement.ConnectionController), cc2);

                var rc2Address = new NetworkAddress("2");
                var rc2 = new RoutingController(rc2Address);
                _controlPlaneElements.Add(new Row(rc2Address, SignallingMessageDestinationControlPlaneElement.RoutingController), rc2);

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

        private void UpdateState(object sender, string state) {
            Console.WriteLine(state);
        }

        private void PassMessage(object sender, SignallingMessage message) {
            var destination = _controlPlaneElements[new Row(message.DestinationAddress, message.DestinationControlPlaneElement)];
            Thread.Sleep(100);
            destination.ReceiveMessage(message);
        }
    }

    internal class Row {
        public NetworkAddress NetworkAddress { get; }
        public SignallingMessageDestinationControlPlaneElement DestinationControlPlaneElement { get; }

        public Row(NetworkAddress networkAddress, SignallingMessageDestinationControlPlaneElement destinationControlPlaneElement) {
            NetworkAddress = networkAddress;
            DestinationControlPlaneElement = destinationControlPlaneElement;
        }

        protected bool Equals(Row other) {
            return Equals(NetworkAddress, other.NetworkAddress) && DestinationControlPlaneElement == other.DestinationControlPlaneElement;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Row)) return false;
            return Equals((Row) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((NetworkAddress != null ? NetworkAddress.GetHashCode() : 0) * 397) ^ (int) DestinationControlPlaneElement;
            }
        }
    }
}