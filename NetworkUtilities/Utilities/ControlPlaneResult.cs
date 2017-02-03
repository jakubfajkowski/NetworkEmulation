using System;
using NetworkUtilities.ControlPlane;

namespace NetworkUtilities.Utilities {
    [Serializable]
    class ControlPlaneResult {
        public string ClientName { get; set; }  
        public SubnetworkPoint Snp { get; set; }
    }
}
