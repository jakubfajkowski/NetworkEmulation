namespace NetworkEmulation {
    public class LogObject {
        public delegate void StateUpdateHandler(object sender, string state);

        public event StateUpdateHandler OnUpdateState;

        protected void UpdateStatus(string state) {
            OnUpdateState?.Invoke(this, state);
        }
    }
}