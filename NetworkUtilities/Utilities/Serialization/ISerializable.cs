using System.Xml.Serialization;

namespace NetworkUtilities.Utilities.Serialization {
    public interface ISerializable : IXmlSerializable {
        UniqueId Id { get; }
    }
}