using System;
using System.Timers;
using NetworkUtilities.Log;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ManagementPlane {
    public class NetworkNodeAgent : LogObject {
        public delegate void ConfigurationHandler(object sender, Link link);

        private readonly ConnectionComponent _managmentPlaneConnectionComponent;

        public NetworkNodeAgent(NetworkAddress networkAddress, string networkManagmentSystemIpAddress,
            int networkManagmentSystemListeningPort) {
            _managmentPlaneConnectionComponent = new ConnectionComponent(networkAddress, networkManagmentSystemIpAddress,
                networkManagmentSystemListeningPort, ConnectionManagerType.NetworkManagementSystem);
            _managmentPlaneConnectionComponent.ObjectReceived += Receive;
            _managmentPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
        }

        private void Receive(object sender, object receivedObject) {
            var message = (ManagementMessage) receivedObject;
            var link = (Link) message.Payload;
            OnUpdateState($"[CONFIGURATION] {link}");
            OnConfigurationReceived(link);
        }

        public event ConfigurationHandler ConfigurationReceived;

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
                Interval = 5000,
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
    }
}