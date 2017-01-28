﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetworkUtilities.ControlPlane;

namespace NetworkUtilities.Log {
    public partial class PathComputationServerLogForm : Form {
        private readonly List<PathComputationServer> _pathComputationServers;
        private readonly Dictionary<NetworkAddress, string> _logDictionary;
        private bool _bound;

        public PathComputationServerLogForm(List<PathComputationServer> pathComputationServers) {
            InitializeComponent();
            _pathComputationServers = pathComputationServers;
            _logDictionary = new Dictionary<NetworkAddress, string>();
        }

        private void UpdateState(object sender, string state) {
            var pathComputationServer = (PathComputationServer)sender;
            var logLine = CreateLogLine(state);

            if (new NetworkAddress(comboBox.Text).Equals(pathComputationServer.NetworkAddress))
                logRichTextBox.AppendText(logLine);

            var logHistory = _logDictionary[pathComputationServer.NetworkAddress];
            _logDictionary[pathComputationServer.NetworkAddress] = string.Concat(logHistory, logLine);
        }

        private string CreateLogLine(string text) {
            return $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] {text}\n";
        }

        private void LogForm_Shown(object sender, EventArgs e) {
            if (!_bound) {
                foreach (var pathComputationServer in _pathComputationServers) {
                    pathComputationServer.UpdateState += (s, state) => BeginInvoke(new Action(() => UpdateState(s, state)));
                    comboBox.Items.Add(pathComputationServer.NetworkAddress);
                    _logDictionary.Add(pathComputationServer.NetworkAddress, "");
                }
                _bound = true;
            }
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                Hide();
            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = (NetworkAddress) comboBox.SelectedItem;
            logRichTextBox.Text = _logDictionary[selected];
        }
    }
}