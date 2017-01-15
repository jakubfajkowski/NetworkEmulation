namespace NetworkUtilities.ControlPlane {
    public enum SignallingMessageOperation {
        //CPCC operations
        CallRequest,
        CallRequestResponse,            // CC       -> NCC
        CallAccept,
        CallAcceptResponse,
        CallTeardown,
        CallTeardownResponse,
        //NCC operations
        DirectoryRequest,
        DirectoryResponse,
        CallCoordination,
        CallCoordinationResponse,
        ConnectionRequest,              // CC(NCC)   -> CC
        ConnectionRequestResponse,      // CC       -> CC(NCC)
        CallConfirmation,     
        //CC operations
        RouteTableQuery,                // CC       -> RC
        RouteTableQueryResponse,        // RC       -> CC
        SetLabels,                      // LRM(NN)  -> CC(NN)
        GetLabelsFromLRM,               // CC(HPCS) -> CC(NN)
        //LRM operations
        GetLabels
        
    }
}