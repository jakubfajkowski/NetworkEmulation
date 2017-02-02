using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetworkUtilities.Utilities.Serialization {
    public static class BinarySerializer {
        public static int PreambuleLength { get; private set; } = sizeof(int);

        public static void SerializeToStream(object anySerializableObject, Stream stream) {
            new BinaryFormatter().Serialize(stream, anySerializableObject);
            stream.Flush();
        }

        public static byte[] Serialize(object anySerializableObject) {
            using (var memoryStream = new MemoryStream()) {
                new BinaryFormatter().Serialize(memoryStream, anySerializableObject);
                return memoryStream.ToArray();
            }
        }

        public static object DeserializeFromStream(Stream stream) {
            return new BinaryFormatter().Deserialize(stream);
        }

        public static object Deserialize(byte[] serializedObject) {
            using (var memoryStream = new MemoryStream(serializedObject)) {
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