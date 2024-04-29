namespace FlowSynx.Cli.Services;

public interface IGitHub
{
    string Organization { get; }
    string FlowSynxRepository { get; }
    string CliRepository { get; }
    string DashboardRepository { get; }
    string FlowSynxArchiveFileName { get; }
    string FlowSynxArchiveHashFileName { get; }
    string DashboardArchiveFileName { get; }
    string DashboardArchiveHashFileName { get; }
    string FlowSynxCliArchiveFileName { get; }
    string FlowSynxCliArchiveHashFileName { get; }

    Task<string> GetLatestVersion(string repositoryName, CancellationToken cancellationToken);
    Task<string> DownloadAsset(string repository, string version, string assetName, string destinationPath, CancellationToken cancellationToken);
    Task<bool> ValidateDownloadedAsset(string path, string repository, string version, string assetName, CancellationToken cancellationToken);
    Task<string> DownloadHashAsset(string repository, string version, string assetHashName, CancellationToken cancellationToken);
}