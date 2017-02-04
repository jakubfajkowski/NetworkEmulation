namespace NetworkUtilities.ControlPlane {
    public enum OperationType {
        //CPCC operations
        CallRequest,
        CallRequestResponse, // CC       -> NCC
        CallAccept,
        CallAcceptResponse,
        CallTeardown,
        CallTeardownResponse,
        //NCC operations
        PolicyRequest,
        DirectoryAddressRequest,
        DirectoryNameRequest,
        DirectorySnppRequest,
        CallCoordination,
        CallCoordinationResponse,
        ConnectionRequest, // CC(NCC)   -> CC
        CallConfirmation,
        CallConfirmationFromNCC,
        //CC operations
        SNPLinkConnectionAllocation,
        SNPLinkConnectionDeallocation,
        RouteTableQuery, // CC       -> RC
        SetLabels, // LRM(NN)  -> CC(NN)
        GetLabelsFromLRM, // CC(HPCS) -> CC(NN)
        ConnectionConfirmation, // CC       -> CC
        PeerCoordination,
        LinkConnectionResponse,
        ConnectionConfirmationToNCC,
        //RC operations 
        NetworkTopology, // RC -> RC
        //LRM operations
        SNPNegotiation,
        SNPNegotiationResponse,
        SNPRelease,
        LocalTopology, 
        Configuration
    }
}