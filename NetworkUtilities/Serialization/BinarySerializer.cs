using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkUtilities.Serialization {
    public static class BinarySerializer {
        public static int PreambuleLength { get; private set; } = sizeof(int);

        public static void SerializeToStream(object anySerializableObject, Stream stream) {
            var serializedObject = Serialize(anySerializableObject);
            var serializedObjectLength = BitConverter.GetBytes(serializedObject.Length);

            var merged = new byte[serializedObjectLength.Length + serializedObject.Length];
            serializedObjectLength.CopyTo(merged, 0);
            serializedObject.CopyTo(merged, serializedObjectLength.Length);

            stream.Write(merged, 0, merged.Length);
        }

        public static byte[] Serialize(object anySerializableObject) {
            using (var memoryStream = new MemoryStream()) {
                new BinaryFormatter().Serialize(memoryStream, anySerializableObject);
                return memoryStream.ToArray();
            }
        }
        public static async Task<object> DeserializeFromStream(Stream stream) {
            var serializedObjectLengthBytes = new byte[PreambuleLength];

            int serializedObjectLength;
            do {
                await stream.ReadAsync(serializedObjectLengthBytes, 0, PreambuleLength);
                serializedObjectLength = BitConverter.ToInt32(serializedObjectLengthBytes, 0);
            } while (serializedObjectLength == 0);

            var serializedObjectBytes = new byte[serializedObjectLength];
            stream.Read(serializedObjectBytes, 0, serializedObjectLength);
            var serializedObject = Deserialize(serializedObjectBytes);
            return serializedObject;
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