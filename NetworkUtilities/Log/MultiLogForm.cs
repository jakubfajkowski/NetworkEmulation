using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Network;

namespace NetworkUtilities.Log {
    public partial class MultiLogForm : Form {
        private readonly Dictionary<LogObject, string> _logDictionary;
        private readonly List<LogObject> _logObjects;

        public MultiLogForm(List<LogObject> logObjects) {
            InitializeComponent();
            _logObjects = logObjects;
            _logDictionary = new Dictionary<LogObject, string>();


            foreach (var logObject in _logObjects) {
                logObject.UpdateState += LogObjectOnUpdateState;

                _logDictionary.Add(logObject, "");
                var listViewItem = new ComboBoxItem {
                    Tag = logObject
                };

                if (logObject is PathComputationServer)
                    listViewItem.Text = ((PathComputationServer) logObject).NetworkAddress.ToString();

                if (logObject is ConnectionManager)
                    listViewItem.Text = ((ConnectionManager)logObject).ConnectionManagerType.ToString();

                comboBox.Items.Add(listViewItem);
            }
        }

        private void LogObjectOnUpdateState(object sender, string state) {
            try {
                BeginInvoke(new Action(() => UpdateState(sender, state)));
            }
            catch (Exception) {
                //ignored
            }
        }

        private void UpdateState(object sender, string state) {
            var logObject = (LogObject) sender;
            var logLine = CreateLogLine(state);

            var comboBoxSelectedItem = (ComboBoxItem) comboBox.SelectedItem;

            if (comboBoxSelectedItem != null) {
                var selectedLogObject = (LogObject)comboBoxSelectedItem.Tag;
                if (selectedLogObject.Equals(logObject))
                    logRichTextBox.AppendText(logLine);
            }


            var logHistory = _logDictionary[logObject];
            _logDictionary[logObject] = string.Concat(logHistory, logLine);
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
            var comboBoxSelectedItem = (ComboBoxItem)comboBox.SelectedItem;

            if (comboBoxSelectedItem != null) {
                var selectedLogObject = (LogObject)comboBoxSelectedItem.Tag;

                logRichTextBox.Text = _logDictionary[selectedLogObject];
            }
        }
    }
}