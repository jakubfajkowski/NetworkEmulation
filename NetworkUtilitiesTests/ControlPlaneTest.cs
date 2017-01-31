using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            _controlPlaneElements.Add(new Row(NameServer.Address, ControlPlaneElementType.Directory), directory);

            var policy = new Policy(NameServer.Address);
            _controlPlaneElements.Add(new Row(NameServer.Address, ControlPlaneElementType.Policy), policy);

            var ncc1Address = new NetworkAddress("1");
            var ncc1 = new NetworkCallController(ncc1Address);
            _controlPlaneElements.Add(new Row(ncc1Address, ControlPlaneElementType.NCC), ncc1);

            var ncc2Address = new NetworkAddress("2");
            var ncc2 = new NetworkCallController(ncc2Address);
            _controlPlaneElements.Add(new Row(ncc2Address, ControlPlaneElementType.NCC), ncc2);

            var cpccAAddress = new NetworkAddress("1.1");
            var cpccA = new CallingPartyCallController(cpccAAddress);
            _controlPlaneElements.Add(new Row(cpccAAddress, ControlPlaneElementType.CPCC), cpccA);

            var cpccBAddress = new NetworkAddress("2.2");
            var cpccB = new CallingPartyCallController(cpccBAddress);
            _controlPlaneElements.Add(new Row(cpccBAddress, ControlPlaneElementType.CPCC), cpccB);

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

        private void UpdateState(object sender, string state) {
            Console.WriteLine(state);
        }

        private void PassMessage(object sender, SignallingMessage message) {
            var destination =
                _controlPlaneElements[new Row(message.DestinationAddress, message.DestinationControlPlaneElement)];
            Thread.Sleep(100);
            destination.ReceiveMessage(message);
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