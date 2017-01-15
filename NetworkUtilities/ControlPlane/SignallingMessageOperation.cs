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
        ConnectionRequest,
        ConnectionRequestResponse,
        CallConfirmation,     
        //CC operations
        ConnectionRequestCC,            // CC       -> CC
        ConnectionRequestResponseCC,    // CC       -> CC
        RouteTableQuery,                // CC       -> RC
        RouteTableQueryResponse,        // RC       -> CC
        SetLabels,                      // LRM(NN)  -> CC(NN)
        GetLabelsFromLRM,               // CC(HPCS) -> CC(NN)
        //LRM operations
        GetLabels
        
    }
}