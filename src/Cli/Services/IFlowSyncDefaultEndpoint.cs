namespace Cli.Services;

public interface IFlowSyncDefaultEndpoint
{
    int GetDefaultHttpPort();
    string GetDefaultHttpEndpoint();
}