using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilities {
    public static class XmlSerializer {
        public static string Serialize(IXmlSerializable obj) {
            var xsSubmit = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            var subReq = obj;
            var xml = "";
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            //settings.OmitXmlDeclaration = true;

            using (var sww = new StringWriter()) {
                using (var writer = XmlWriter.Create(sww, settings)) {
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

            using (var xmlReader = XmlReader.Create(textReader, settings)) {
                readingObject.ReadXml(xmlReader);
            }
        }

        public static string Serialize(object obj) {
            using (var sww = new StringWriter()) {
                var xml = new System.Xml.Serialization.XmlSerializer(obj.GetType());
                xml.Serialize(sww, obj);
                return sww.ToString();
            }
        }

        public static object Deserialize(string serializedObject, Type objectType) {
            using (var sr = new StringReader(serializedObject)) {
                var xml = new System.Xml.Serialization.XmlSerializer(objectType);
                return xml.Deserialize(sr);
            }
        }

        public static void Serialize<T>(XmlWriter writer, T obj) {
            var xml = new System.Xml.Serialization.XmlSerializer(typeof(T));
            xml.Serialize(writer, obj);
        }

        public static T Deserialize<T>(XmlReader reader) {
            var xml = new System.Xml.Serialization.XmlSerializer(typeof(T));

            var result =  (T) xml.Deserialize(reader) ;

            return result;
        }
    }
}