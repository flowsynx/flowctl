using EnsureThat;
using FlowCtl.Common;
using FlowCtl.Services.Abstracts;
using FlowSynx.Environment;
using FlowSynx.IO;
using FlowSynx.IO.Serialization;
using FlowSynx.Security;

namespace FlowCtl.Services.Concretes;

public class GitHub : IGitHub
{
    private readonly IOperatingSystemInfo _operatingSystemInfo;
    private readonly IDeserializer _deserializer;

    public GitHub(IOperatingSystemInfo operatingSystemInfo, IDeserializer deserializer)
    {
        EnsureArg.IsNotNull(operatingSystemInfo, nameof(operatingSystemInfo));
        EnsureArg.IsNotNull(deserializer, nameof(deserializer));

        _operatingSystemInfo = operatingSystemInfo;
        _deserializer = deserializer;
    }

    public string Organization => "flowsynx";
    public string FlowSynxRepository => "flowsynx";
    public string FlowCtlRepository => "flowctl";
    public string DashboardRepository => "dashboard";
    public string FlowSynxArchiveFileName => $"flowsynx-{ArchiveName.ToLower()}";
    public string FlowSynxArchiveHashFileName => $"flowsynx-{ArchiveName.ToLower()}.{HashFileExtension}";
    public string DashboardArchiveFileName => $"dashboard-{ArchiveName.ToLower()}";
    public string DashboardArchiveHashFileName => $"dashboard-{ArchiveName.ToLower()}.{HashFileExtension}";
    public string FlowCtlArchiveFileName => $"flowctl-{ArchiveName.ToLower()}";
    public string FlowCtlArchiveHashFileName => $"flowctl-{ArchiveName.ToLower()}.{HashFileExtension}";
    private string ArchiveName => $"{_operatingSystemInfo.Type}-{_operatingSystemInfo.Architecture}.{CompressionFileExtension}";
    private string CompressionFileExtension => string.Equals(_operatingSystemInfo.Type, "windows", StringComparison.OrdinalIgnoreCase) ? "zip" : "tar.gz";
    private string HashFileExtension => "sha256";


    public async Task<string> GetLatestVersion(string repositoryName, CancellationToken cancellationToken)
    {
        var httpClient = new HttpClient();
        var uri = $"https://api.github.com/repos/{Organization}/{repositoryName}/tags";
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(uri)
        };

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var tags = _deserializer.Deserialize<List<GitHubTag>>(responseBody);
        return tags.Count > 0 ? tags[0].Name : string.Empty;
    }

    public async Task<string> DownloadAsset(string repository, string version, string assetName, string destinationPath, CancellationToken cancellationToken)
    {
        var uri = $"https://github.com/{Organization}/{repository}/releases/download/v{version}/{assetName}";
        var stream = await NetHelper.DownloadFile(uri, cancellationToken);
        var path = Path.Combine(destinationPath, assetName);
        stream.WriteTo(path);
        return path;
    }

    public async Task<bool> ValidateDownloadedAsset(string path, string repository, string version, string assetName, CancellationToken cancellationToken)
    {
        var expectedHash = await DownloadHashAsset(repository, version, assetName, cancellationToken);
        var downloadHash = HashHelper.Sha256.GetHash(new FileInfo(path));

        if (string.Equals(downloadHash.Trim(), expectedHash.Trim(), StringComparison.CurrentCultureIgnoreCase))
            return true;

        File.Delete(path);
        return false;
    }

    public async Task<string> DownloadHashAsset(string repository, string version, string assetHashName, CancellationToken cancellationToken)
    {
        var uri = $"https://github.com/{Organization}/{repository}/releases/download/v{version}/{assetHashName}";
        var stream = await NetHelper.DownloadFile(uri, cancellationToken);
        return await GetAssetHashCode(stream, cancellationToken);
    }

    private async Task<string> GetAssetHashCode(Stream stream, CancellationToken cancellationToken)
    {
        using var sr = new StreamReader(stream);
        var content = await sr.ReadToEndAsync(cancellationToken);
        return content.Split('*')[0].Trim();
    }
}

internal class GitHubTag
{
    public required string Name { get; set; }
}