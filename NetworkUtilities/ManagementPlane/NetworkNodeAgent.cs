using System;
using System.Timers;
using NetworkUtilities.Log;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ManagementPlane {
    public class NetworkNodeAgent : LogObject {
        public delegate void ConfigurationHandler(object sender, Link link);
        public delegate void ConnectClientHandler(object sender, Link linkIn, Link linkOut, NetworkAddress clientAddress);

        private readonly ConnectionComponent _managmentPlaneConnectionComponent;

        public NetworkNodeAgent(NetworkAddress networkAddress, string networkManagmentSystemIpAddress,
            int networkManagmentSystemListeningPort) {
            _managmentPlaneConnectionComponent = new ConnectionComponent(networkAddress, networkManagmentSystemIpAddress,
                networkManagmentSystemListeningPort, ConnectionManagerType.NetworkManagementSystem);
            _managmentPlaneConnectionComponent.ObjectReceived += Receive;
            _managmentPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
        }

        private void Receive(object sender, object receivedObject) {
            var message = (ManagementMessage)receivedObject;

            switch (message.Type) {
                case ManagementMessageType.Configuration:
                    HandleConfiguration(message);
                    break;

                case ManagementMessageType.ConnectClient:
                    HandleConnectClient(message);
                    break;
            }
        }

        private void HandleConfiguration(ManagementMessage message) {
            var link = (Link)message.Payload;
            OnUpdateState($"[CONFIGURATION] {link}");
            OnConfigurationReceived(link);
        }

        private void HandleConnectClient(ManagementMessage message) {
            var payload = (object[]) message.Payload;
            var linkIn = (Link) payload[0];
            var linkOut = (Link) payload[1];
            var clientAddress = (NetworkAddress) payload[2];

            OnUpdateState($"[CONNECT_CLIENT] [IN]  {clientAddress} {linkIn}");
            OnUpdateState($"[CONNECT_CLIENT] [OUT]{clientAddress} {linkOut}");
            OnConnectClientReceived(linkIn, linkOut, clientAddress);
        }

        public event ConfigurationHandler ConfigurationReceived;
        public event ConnectClientHandler ConnectClientReceived;

        protected virtual void OnConfigurationReceived(Link link) {
            ConfigurationReceived?.Invoke(this, link);
        }

        public void Initialize() {
            _managmentPlaneConnectionComponent.Initialize();
            StartSendingKeepAliveMessages();
        }

        private void StartSendingKeepAliveMessages() {
            var timer = new Timer {
                AutoReset = true,
                Interval = 10000,
                Enabled = true
            };

            timer.Elapsed += OnTimedEvent;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e) {
            if (_managmentPlaneConnectionComponent.Online) {
                var message = new ManagementMessage(ManagementMessageType.KeepAlive, new object());
                _managmentPlaneConnectionComponent.Send(message);
                //OnUpdateState("[SENT] [KEEP_ALIVE]");
            }
        }

        protected virtual void OnConnectClientReceived(Link linkIn, Link linkOut, NetworkAddress clientAddress) {
            ConnectClientReceived?.Invoke(this, linkIn, linkOut, clientAddress);
        }
    }
}