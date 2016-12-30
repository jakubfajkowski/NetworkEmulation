using System.Xml.Serialization;

namespace NetworkUtilities {
    public interface ISerializable : IXmlSerializable {
        UniqueId Id { get; }
    }
}