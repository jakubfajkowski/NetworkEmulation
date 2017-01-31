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
        PolicyResponse,
        DirectoryAddressRequest,
        DirectoryAddressResponse,
        DirectoryNameRequest,
        DirectoryNameResponse,
        DirectorySnppRequest,
        DirectorySnppResponse,
        CallCoordination,
        CallCoordinationResponse,
        ConnectionRequest, // CC(NCC)   -> CC
        ConnectionRequestResponse, // CC       -> CC(NCC)
        CallConfirmation,
        CallConfirmationFromNCC,
        //CC operations
        SNPLinkConnectionRequest,
        SNPLinkConnectionDeallocation,
        RouteTableQuery, // CC       -> RC
        RouteTableQueryResponse, // RC       -> CC
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