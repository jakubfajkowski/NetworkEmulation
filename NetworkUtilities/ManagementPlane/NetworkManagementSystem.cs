﻿using System;
using System.Collections.Generic;
using System.Timers;
using NetworkUtilities.ControlPlane.GraphAlgorithm;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ManagementPlane {
    public class NetworkManagementSystem : ConnectionManager {
        private const int MaxTimeNotReceivingKeepAliveMessage = 30000;

        private readonly Dictionary<NetworkAddress, DateTime> _keepAliveDictionary =
            new Dictionary<NetworkAddress, DateTime>();

        public NetworkManagementSystem(int listeningPort) : base(listeningPort, ConnectionManagerType.NetworkManagementSystem) {
            CreateKeepAliveTimer();
        }

        private void CreateKeepAliveTimer() {
            var timer = new Timer {
                AutoReset = true,
                Interval = MaxTimeNotReceivingKeepAliveMessage / 2,
                Enabled = true
            };

            timer.Elapsed += OnTimedEvent;
        }

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress inputNetworkAddress) {
            var message = (ManagementMessage) receivedObject;

            switch (message.Type) {
                    case ManagementMessageType.KeepAlive:
                    HandleKeepAlive(inputNetworkAddress);
                    break;
            }
        }

        private void HandleKeepAlive(NetworkAddress inputNetworkAddress) {
            if (!_keepAliveDictionary.ContainsKey(inputNetworkAddress)) {
                OnUpdateState($"[ONLINE] {inputNetworkAddress}");
                _keepAliveDictionary.Add(inputNetworkAddress, DateTime.Now);
            }

            _keepAliveDictionary[inputNetworkAddress] = DateTime.Now;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e) {
            var keysToRemove = new List<NetworkAddress>();

            try {
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
            catch (InvalidOperationException) {
                //Ignored
            }
        }

        public void SendConfigurationMessage(Link link, NetworkAddress destinationAddress) {
            var message = new ManagementMessage(ManagementMessageType.Configuration, link);
            Send(message, destinationAddress);
            OnUpdateState($"[CONFIGURATION] {link}");
        }
    }
}