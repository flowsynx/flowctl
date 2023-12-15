namespace Cli.Core.Services;

public interface IFlowSyncDefaultEndpoint
{
    int GetDefaultHttpPort();
    string GetDefaultHttpEndpoint();
}