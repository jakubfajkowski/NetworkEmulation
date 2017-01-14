using System;
using System.Windows.Forms;

namespace NetworkUtilities.Log {
    public partial class LogForm : Form {
        private readonly LogObject _logObject;
        private bool _bound;

        public LogForm(LogObject logObject) {
            InitializeComponent();
            _logObject = logObject;
        }

        private void UpdateState(object sender, string state) {
            logRichTextBox.AppendText(CreateLogLine(state));
        }

        private string CreateLogLine(string text) {
            return $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] {text}\n";
        }

        private void LogForm_Shown(object sender, EventArgs e) {
            if (!_bound) {
                _logObject.OnUpdateState += (s, state) => BeginInvoke(new Action(() => UpdateState(s, state)));
                _bound = true;
            }
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                Hide();
            }
        }
    }
}