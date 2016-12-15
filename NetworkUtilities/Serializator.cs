using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkUtilities {
    public static class Serializator {
        public static byte[] Serialize(object anySerializableObject) {
            using (var memoryStream = new MemoryStream()) {
                new BinaryFormatter().Serialize(memoryStream, anySerializableObject);
                return memoryStream.ToArray();
            }
        }

        public static object Deserialize(byte[] data) {
            using (var memoryStream = new MemoryStream(data)) {
                return new BinaryFormatter().Deserialize(memoryStream);
            }
        }
    }
}