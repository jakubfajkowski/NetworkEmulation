using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using NetworkEmulation.Editor.Element;
using NetworkUtilities;
using NetworkUtilities.Network;

namespace NetworkEmulation.Network {
    public class NetworkManagmentSystem : ConnectionManager {
        private const int MaxTimeNotReceivingKeepAliveMessage = 5000;
        private readonly Dictionary<NetworkAddress, Timer> _keepAliveDictionary = new Dictionary<NetworkAddress, Timer>();

        public NetworkManagmentSystem(int listeningPort) : base(listeningPort) {}

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress networkAddress) {
            if (_keepAliveDictionary.ContainsKey(networkAddress)) {
                _keepAliveDictionary[networkAddress].Stop();
                _keepAliveDictionary[networkAddress].Start();
            }
            else {
                OnUpdateState(networkAddress + " is online");
                _keepAliveDictionary[networkAddress] = CreateKeepAliveMessagesTimer();
            }
        }

        private Timer CreateKeepAliveMessagesTimer() {
            var timer = new Timer {
                AutoReset = true,
                Interval = MaxTimeNotReceivingKeepAliveMessage,
                Enabled = true
            };

            timer.Elapsed += OnTimedEvent;
            
            return timer;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e) {
            var timer = (Timer) source;
            timer.Stop();
            var recordToRemove = _keepAliveDictionary.First(kvp => kvp.Value.Equals(timer));
            _keepAliveDictionary.Remove(recordToRemove.Key);
            timer.Dispose();
            OnUpdateState(recordToRemove.Key + " is offline");
        }

        public bool IsNetworkNodeOnline(NetworkAddress networkAddress) {
            return _keepAliveDictionary.ContainsKey(networkAddress);
        }

        public bool AreOnline(List<NetworkNodeView> networkNodeViews) {
            foreach (var networkNode in networkNodeViews) {
                if (!IsNetworkNodeOnline(networkNode.NetworkAddress)) return false;
            }
            return true;
        }
    }
}