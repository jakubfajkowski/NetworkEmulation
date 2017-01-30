using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.Log {
    public partial class PathComputationServerLogForm : Form {
        private readonly Dictionary<NetworkAddress, string> _logDictionary;
        private readonly List<PathComputationServer> _pathComputationServers;
        private bool _bound;

        public PathComputationServerLogForm(List<PathComputationServer> pathComputationServers) {
            InitializeComponent();
            _pathComputationServers = pathComputationServers;
            _logDictionary = new Dictionary<NetworkAddress, string>();


            foreach (var pathComputationServer in _pathComputationServers) {
                pathComputationServer.UpdateState += (s, state) => BeginInvoke(new Action(() => UpdateState(s, state)));
                comboBox.Items.Add(pathComputationServer.NetworkAddress);
                _logDictionary.Add(pathComputationServer.NetworkAddress, "");
            }
        }

        private void UpdateState(object sender, string state) {
            var pathComputationServer = (PathComputationServer) sender;
            var logLine = CreateLogLine(state);

            if (new NetworkAddress(comboBox.Text).Equals(pathComputationServer.NetworkAddress))
                logRichTextBox.AppendText(logLine);

            var logHistory = _logDictionary[pathComputationServer.NetworkAddress];
            _logDictionary[pathComputationServer.NetworkAddress] = string.Concat(logHistory, logLine);
        }

        private string CreateLogLine(string text) {
            var datetime = DateTime.Now;
            return $"[{datetime}.{datetime.Millisecond}] {text}\n";
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