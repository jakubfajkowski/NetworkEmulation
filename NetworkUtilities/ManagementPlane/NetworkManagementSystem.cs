using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ManagementPlane {
    public class NetworkManagementSystem : ConnectionManager {
        private const int MaxTimeNotReceivingKeepAliveMessage = 5000;

        private readonly Dictionary<NetworkAddress, DateTime> _keepAliveDictionary =
            new Dictionary<NetworkAddress, DateTime>();

        public NetworkManagementSystem(int listeningPort) : base(listeningPort, ConnectionManagerType.NetworkManagementSystem) {
            CreateKeepAliveTimer();
        }

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress inputNetworkAddress) {
            if (!_keepAliveDictionary.ContainsKey(inputNetworkAddress)) {
                OnUpdateState($"[ONLINE] {inputNetworkAddress}");
                _keepAliveDictionary.Add(inputNetworkAddress, DateTime.Now);
            }
                
            _keepAliveDictionary[inputNetworkAddress] = DateTime.Now;
        }

        private void CreateKeepAliveTimer() {
            var timer = new Timer {
                AutoReset = true,
                Interval = MaxTimeNotReceivingKeepAliveMessage,
                Enabled = true
            };

            timer.Elapsed += OnTimedEvent;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e) {
            var keysToRemove = new List<NetworkAddress>();

            foreach (var pair in _keepAliveDictionary) {
                if (DateTime.Now.Subtract(pair.Value).TotalMilliseconds > MaxTimeNotReceivingKeepAliveMessage) {
                    keysToRemove.Add(pair.Key);
                }
            }

            foreach (var key in keysToRemove) {
                _keepAliveDictionary.Remove(key);
                OnUpdateState($"[OFFLINE] {key}");
            }
        }

        public bool IsNetworkNodeOnline(NetworkAddress networkAddress) {
            return _keepAliveDictionary.ContainsKey(networkAddress);
        }

        public bool AreOnline(List<NetworkAddress> networkAddresses) {
            foreach (var address in networkAddresses) if (!IsNetworkNodeOnline(address)) return false;
            return true;
        }
    }
}