using System;
using System.Xml.Serialization;

namespace NetworkUtilities {
    [Serializable, XmlRoot]
    public class UniqueId {
        [XmlAttribute]
        public string Value { get; set; }
        
        private UniqueId() {
        }

        public UniqueId(string value) {
            Value = value;
        }

        public static UniqueId Generate() {
            return new UniqueId {
                Value = new System.Xml.UniqueId().ToString()
            };
        }

        public override string ToString() {
            return Value;
        }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != typeof(UniqueId)) return false;
            return Value.Equals(obj.ToString());
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
    }
}
