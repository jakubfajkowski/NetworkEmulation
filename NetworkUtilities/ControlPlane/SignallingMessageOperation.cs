namespace NetworkUtilities.ControlPlane {
    public enum SignallingMessageOperation {
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
        RouteTableQuery, // CC       -> RC
        RouteTableQueryResponse, // RC       -> CC
        SetLabels, // LRM(NN)  -> CC(NN)
        GetLabelsFromLRM, // CC(HPCS) -> CC(NN)
        ConnectionConfirmation, // CC       -> CC
        PeerCoordination,
        LinkConnectionRequest,
        ConnectionConfirmationToNCC,
        //RC operations 
        NetworkTopology, // RC -> RC
        //LRM operations
        SNPNegotiation,
        SNPNegotiationResponse,
        SNPRelease,
        Confirm,
        GetLabels,
        LocalTopology // LRM -> RC
    }
}