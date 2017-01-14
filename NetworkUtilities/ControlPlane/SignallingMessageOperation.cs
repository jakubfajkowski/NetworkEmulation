namespace NetworkUtilities.ControlPlane {
    public enum SignallingMessageOperation {
        LocalTopology,
        DirectoryRequest,
        DirectoryResponse,
        CallRequest,
        CallCoordination,
        CallTeardown,
        ConnectionRequest,
        CallAccept
    }
}