using System;
using System.Linq;
using System.Windows.Forms.VisualStyles;

namespace NetworkUtilities {
    public class NetworkAddress {
        private const char Separator = '.';
        private readonly string _value;
        public int Levels { get; private set; }

        public NetworkAddress(string value) {
            _value = value;
            Levels = _value.Split(Separator).Length;
        }

        public static NetworkAddress Append(NetworkAddress networkAddress, int number) {
            var value = networkAddress._value + Separator + number;
            return new NetworkAddress(value);
        }

        public int GetId(int level) {
            var ids = _value.Split(Separator);

            return int.Parse(ids[level]);
        }

        public int GetLastId() {
            var ids = _value.Split(Separator);

            return int.Parse(ids[ids.Length - 1]);
        }

        public int GetParentsId() {
            var ids = _value.Split(Separator);

            return int.Parse(ids[ids.Length - 2]);
        }

        public NetworkAddress GetParentsAddress() {
            var ids = _value.Split(Separator);
            var value = string.Join(Separator.ToString(), ids.Take(ids.Length - 2));
            return new NetworkAddress(value);
        }

        public NetworkAddress GetRootFromBeggining(int level) {
            var ids = _value.Split(Separator);
            var value = string.Join(Separator.ToString(), ids.Take(level));
            return new NetworkAddress(value);
        }

        public NetworkAddress GetRootFromEnd(int level) {
            var ids = _value.Split(Separator);
            var value = string.Join(Separator.ToString(), ids.Take(ids.Length - level));
            return new NetworkAddress(value);
        }

        public override string ToString() {
            return _value;
        }

        public override bool Equals(object obj) {
            return _value.Equals(obj);
        }

        public override int GetHashCode() {
            return _value.GetHashCode();
        }
    }
}
