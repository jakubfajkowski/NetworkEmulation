using System;
using System.Windows.Forms;

namespace NetworkEmulation {
    public partial class LogForm : Form {
        private readonly LogObject _logObject;

        public LogForm(ref LogObject logObject) {
            InitializeComponent();
            _logObject = logObject;
            _logObject.OnUpdateState += UpdateState;
        }

        private void UpdateState(object sender, string state) {
            logRichTextBox.Text += CreateLogLine(state);
        }

        private string CreateLogLine(string text) {
            return $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] {text}\n";
        }

        private void LogForm_FormClosed(object sender, FormClosedEventArgs e) {
            _logObject.OnUpdateState -= UpdateState;
        }
    }
}