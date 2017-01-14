namespace NetworkUtilities {
    class NetworkAddress {
        private readonly string _value;

        public NetworkAddress(string value) {
            _value = value;
        }

        public static NetworkAddress Append(NetworkAddress networkAddress, int number) {
            var value = networkAddress._value + "." + number;
            return new NetworkAddress(value);
        }

        public int GetId(int level) {
            var ids = _value.Split('.');

            return int.Parse(ids[level - 1]);
        }

        public int GetLastId() {
            var ids = _value.Split('.');

            return int.Parse(ids[ids.Length - 1]);
        }

        public override string ToString() {
            return _value;
        }
    }
}
