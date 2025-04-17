using FlowCtl.Core.Services.Github;
using Octokit;
using System.Security.Cryptography;

namespace FlowCtl.Infrastructure.Services.Github;

public class GitHubReleaseManager : IGitHubReleaseManager
{
    private readonly IGitHubClient _client;
    private readonly HttpClient _httpClient;

    public GitHubReleaseManager(IGitHubClient client, HttpClient httpClient)
    {
        _client = client;
        _httpClient = httpClient;
    }

    public async Task<string> GetLatestVersion(string owner, string repository)
    {
        var release = await _client.Repository.Release.GetLatest(owner, repository);
        return release.TagName;
    }

    public async Task<string> DownloadAsset(string owner, string repository, string assetName,
        string downloadPath, string? versionTag = null)
    {
        var release = await GetReleaseByVersionOrLatest(owner, repository, versionTag);
        var asset = release.Assets.FirstOrDefault(a => a.Name == assetName)
            ?? throw new Exception($"Asset '{assetName}' not found in release '{release.TagName}'.");

        var response = await _httpClient.GetAsync(asset.BrowserDownloadUrl);
        response.EnsureSuccessStatusCode();

        var parentDirectory = Path.GetDirectoryName(downloadPath);
        if (!string.IsNullOrWhiteSpace(parentDirectory))
            Directory.CreateDirectory(parentDirectory);

        await using var fs = new FileStream(downloadPath, System.IO.FileMode.Create);
        await response.Content.CopyToAsync(fs);

        return downloadPath;
    }

    public async Task<string> DownloadHashAsset(string owner, string repository, string hashAssetName,
        string downloadPath, string? versionTag = null)
    {
        return await DownloadAsset(owner, repository, hashAssetName, downloadPath, versionTag);
    }

    public string GetAssetHashCode(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using HashAlgorithm hasher = SHA256.Create();
        var hashBytes = hasher.ComputeHash(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    public bool ValidateDownloadedAsset(string downloadedFilePath, string hashFilePath)
    {
        var expectedHash = File.ReadAllText(hashFilePath).Trim().Split(' ').FirstOrDefault()?.ToLowerInvariant();
        var actualHash = GetAssetHashCode(downloadedFilePath);

        return string.Equals(expectedHash, actualHash, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<Release> GetReleaseByVersionOrLatest(string owner, string repository,
        string? versionTag = null)
    {
        if (!string.IsNullOrWhiteSpace(versionTag))
            return await _client.Repository.Release.Get(owner, repository, versionTag);

        return await _client.Repository.Release.GetLatest(owner, repository);
    }
}