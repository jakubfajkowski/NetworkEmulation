using System.Xml.Serialization;

namespace NetworkUtilities.element {
    public class SerializableParameters {
        public SerializableParameters() {
            Count++;
        }

        private static int Count { get; set; }

        [XmlAttribute]
        public int Id { get; set; }
    }
}