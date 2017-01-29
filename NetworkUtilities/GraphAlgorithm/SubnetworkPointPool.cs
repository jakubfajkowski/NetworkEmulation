using System.Collections.Generic;

namespace NetworkUtilities.GraphAlgorithm {
    public class SubnetworkPointPool {
        public SubnetworkPointPool() {
            Id = 0;
            LinksOut = new Link[0];
        }

        public SubnetworkPointPool(int id) {
            Id = id;
            LinksOut = new Link[0];
        }

        public NetworkAddress NetworkSnppAddress { get; set; }
        public int PortSnpp { get; private set; }
        public List<string> Snp { get; set; }

        public int Id { get; set; }

        public SubnetworkPointPool Prev { get; set; }

        public double CumulatedWeight { get; set; }

        public Link[] LinksOut { get; private set; }

        protected bool Equals(SubnetworkPointPool other) {
            return Equals(NetworkSnppAddress, other.NetworkSnppAddress) && PortSnpp == other.PortSnpp && Id == other.Id;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SubnetworkPointPool) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = NetworkSnppAddress != null ? NetworkSnppAddress.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ PortSnpp;
                hashCode = (hashCode * 397) ^ Id;
                return hashCode;
            }
        }

        public void AddEdgeOut(Link link) {
            var tmpLinks = new Link[LinksOut.Length + 1];
            for (var i = 0; i < LinksOut.Length; i++)
                tmpLinks[i] = LinksOut[i];
            tmpLinks[LinksOut.Length] = link;
            LinksOut = tmpLinks;
        }
    }
}