namespace FlowCtl.Core.Github;

public interface IGitHubReleaseManager
{
    Task<string> GetLatestVersion(string owner, string repository);
    Task<string> DownloadAsset(string owner, string repository, string assetName, string downloadPath, string? versionTag = null);
    Task<string> DownloadHashAsset(string owner, string repository, string hashAssetName, string downloadPath, string? versionTag = null);
    string GetAssetHashCode(string filePath);
    bool ValidateDownloadedAsset(string downloadedFilePath, string hashFilePath);
}