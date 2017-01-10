using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkUtilities.Serialization {
    public static class BinarySerializer {
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

        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false) {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create)) {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public static T ReadFromBinaryFile<T>(string filePath) {
            using (Stream stream = File.Open(filePath, FileMode.Open)) {
                var binaryFormatter = new BinaryFormatter();
                return (T) binaryFormatter.Deserialize(stream);
            }
        }
    }
}