using System;
using System.Windows.Forms;

namespace NetworkUtilities.Log {
    public partial class LogForm : Form {
        private readonly LogObject _logObject;
        private bool _bound;

        public LogForm(LogObject logObject) {
            InitializeComponent();
            _logObject = logObject;
            _logObject.UpdateState += (s, state) => BeginInvoke(new Action(() => UpdateState(s, state)));
        }

        private void UpdateState(object sender, string state) {
            logRichTextBox.AppendText(CreateLogLine(state));
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
    }
}