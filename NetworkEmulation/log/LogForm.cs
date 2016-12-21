using System;
using System.Windows.Forms;

namespace NetworkEmulation.log {
    public partial class LogForm : Form {
        private readonly LogObject _logObject;

        public LogForm(LogObject logObject) {
            InitializeComponent();
            _logObject = logObject;
        }

        private void UpdateState(object sender, string state) {
            logRichTextBox.Text += CreateLogLine(state);
        }

        private string CreateLogLine(string text) {
            return $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] {text}\n";
        }

        private void LogForm_Shown(object sender, EventArgs e) {
            _logObject.OnUpdateState += (s, state) => BeginInvoke(new Action(() => UpdateState(s, state)));
        }
    }
}