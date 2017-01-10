namespace NetworkEmulation.Log {
    public class LogObject {
        public delegate void StateUpdateHandler(object sender, string state);

        public event StateUpdateHandler OnUpdateState;

        protected void UpdateState(string state) {
            OnUpdateState?.Invoke(this, state);
        }
    }
}