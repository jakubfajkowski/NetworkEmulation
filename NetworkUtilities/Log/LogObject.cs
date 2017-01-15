namespace NetworkUtilities.Log {
    public class LogObject {
        public delegate void MessageHandler(object sender, string state);

        public event MessageHandler OnUpdateState;

        protected void UpdateState(string state) {
            OnUpdateState?.Invoke(this, state);
        }
    }
}