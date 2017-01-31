namespace NetworkUtilities.Log {
    public class LogObject {
        public delegate void MessageHandler(object sender, string state);

        public event MessageHandler UpdateState;

        protected virtual void OnUpdateState(string state) {
            UpdateState?.Invoke(this, state);
        }
    }
}