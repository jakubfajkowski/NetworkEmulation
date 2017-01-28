using System.Collections.Generic;

namespace NetworkUtilities.GraphAlgorithm
{
    public class SubnetworkPointPool
    {
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
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubnetworkPointPool) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (NetworkSnppAddress != null ? NetworkSnppAddress.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ PortSnpp;
                hashCode = (hashCode*397) ^ Id;
                return hashCode;
            }
        }

        public void AddEdgeOut(Link link)
        {
            Link[] tmp_links = new Link[LinksOut.Length + 1];
            for (int i = 0; i < LinksOut.Length; i++)
                tmp_links[i] = LinksOut[i];
            tmp_links[LinksOut.Length] = link;
            LinksOut = tmp_links;
        }

        public SubnetworkPointPool()
        {
            this.Id = 0;
            this.LinksOut = new Link[0];
        }

        public SubnetworkPointPool(int id)
        {
            this.Id = id;
            this.LinksOut = new Link[0];
        }
    }
}
