namespace NetworkUtilities.ControlPlane {
    public enum SignallingMessageOperation {
        //CPCC operations
        CallRequest,
        CallRequestResponse,
        CallAccept,
        CallAcceptResponse,
        CallTeardown,
        CallTeardownResponse,
        //NCC operations
        DirectoryRequest,
        DirectoryResponseAddress,
        DirectoryResponseName,
        CallCoordination,
        CallCoordinationResponse,
        ConnectionRequest,
        ConnectionRequestResponse,
        CallConfirmation,     
        //CC operations
        ConnectionRequestCC, // NCC do CC
        RouteTableQuery, // CC do RC???
        SetLabels, // LRM(NN) do CC(NN)
        GetLabelsFromLRM, // CC(HPCS) do CC(NN)
        //LRM operations
        GetLabels
        
    }
}