using System.Xml.Serialization;

namespace NetworkUtilities.Serialization {
    public interface ISerializable : IXmlSerializable {
        UniqueId Id { get; }
    }
}