using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilities {
    public static class XmlSerializator {
        public static string Serialize(IXmlSerializable obj) {
            XmlSerializer xsSubmit = new XmlSerializer(obj.GetType());
            var subReq = obj;
            var xml = "";
            var settings = new XmlWriterSettings();
            //settings.Indent = true;

            using (var sww = new StringWriter()) {
                using (XmlWriter writer = XmlWriter.Create(sww, settings)) {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString(); // Your XML
                }
            }

            return xml;
        }

        public static void Deserialize(IXmlSerializable readingObject, string serializedObject) {
            TextReader textReader = new StringReader(serializedObject);
            var settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            using (XmlReader xmlReader = XmlReader.Create(textReader, settings)) {
                readingObject.ReadXml(xmlReader);
            }
        }

        public static string Serialize(object obj) {
            using (var sww = new StringWriter()) {
                var xml = new XmlSerializer(obj.GetType());
                xml.Serialize(sww, obj);
                return sww.ToString();
            }
        }

        public static object Deserialize(string serializedObject, Type objectType) {
            using (var sr = new StringReader(serializedObject)) {
                var xml = new XmlSerializer(objectType);
                return xml.Deserialize(sr);
            }
        }
    }
}
