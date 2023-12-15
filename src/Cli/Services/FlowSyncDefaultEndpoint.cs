namespace Cli.Services;

public class FlowSyncDefaultEndpoint : IFlowSyncDefaultEndpoint
{
    private readonly IEnvironmentVariablesManager _environmentVariablesManager;

    public FlowSyncDefaultEndpoint(IEnvironmentVariablesManager environmentVariablesManager)
    {
        _environmentVariablesManager = environmentVariablesManager;
    }

    public int GetDefaultHttpPort()
    {
        var flowSyncPort = _environmentVariablesManager.Get("FLOWSYNC_HTTP_PORT");
        var parsedPort = int.TryParse(flowSyncPort, out var result);
        return result > 0 ? result : 5860;
    }

    string IFlowSyncDefaultEndpoint.GetDefaultHttpEndpoint()
    {
        var port = GetDefaultHttpPort();
        return $"http://127.0.0.1:{port}";
    }
}