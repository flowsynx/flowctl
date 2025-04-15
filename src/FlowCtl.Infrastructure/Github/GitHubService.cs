//using EnsureThat;
//using FlowCtl.Core.Serialization;
//using FlowCtl.Core.Services;
//using System.Net;
//using System.Resources;
//using System.Security.Cryptography;

//namespace FlowCtl.Infrastructure.Services;

//public class GitHubService : IGitHubService
//{
//    private readonly IOperatingSystemInfo _operatingSystemInfo;
//    private readonly IJsonDeserializer _deserializer;

//    public GitHubService(IOperatingSystemInfo operatingSystemInfo, IJsonDeserializer deserializer)
//    {
//        EnsureArg.IsNotNull(operatingSystemInfo, nameof(operatingSystemInfo));
//        EnsureArg.IsNotNull(deserializer, nameof(deserializer));

//        _operatingSystemInfo = operatingSystemInfo;
//        _deserializer = deserializer;
//    }

//    public string Organization => "flowsynx";
//    public string FlowSynxRepository => "flowsynx";
//    public string FlowCtlRepository => "flowctl";
//    public string DashboardRepository => "dashboard";
//    public string FlowSynxArchiveFileName => $"flowsynx-{ArchiveName.ToLower()}";
//    public string FlowSynxArchiveHashFileName => $"flowsynx-{ArchiveName.ToLower()}.{HashFileExtension}";
//    public string DashboardArchiveFileName => $"dashboard-{ArchiveName.ToLower()}";
//    public string DashboardArchiveHashFileName => $"dashboard-{ArchiveName.ToLower()}.{HashFileExtension}";
//    public string FlowCtlArchiveFileName => $"flowctl-{ArchiveName.ToLower()}";
//    public string FlowCtlArchiveHashFileName => $"flowctl-{ArchiveName.ToLower()}.{HashFileExtension}";
//    private string ArchiveName => $"{_operatingSystemInfo.Type}-{_operatingSystemInfo.Architecture}.{CompressionFileExtension}";
//    private string CompressionFileExtension => string.Equals(_operatingSystemInfo.Type, "windows", StringComparison.OrdinalIgnoreCase) ? "zip" : "tar.gz";
//    private string HashFileExtension => "sha256";


//    public async Task<string> GetLatestVersion(string repositoryName, CancellationToken cancellationToken)
//    {
//        var httpClient = new HttpClient();
//        var uri = $"https://api.github.com/repos/{Organization}/{repositoryName}/tags";
//        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
//        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

//        var request = new HttpRequestMessage
//        {
//            Method = HttpMethod.Get,
//            RequestUri = new Uri(uri)
//        };

//        var response = await httpClient.SendAsync(request, cancellationToken);
//        response.EnsureSuccessStatusCode();

//        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
//        var tags = _deserializer.Deserialize<List<GitHubTag>>(responseBody);
//        return tags.Count > 0 ? tags[0].Name : string.Empty;
//    }

//    public async Task<string> DownloadAsset(string repository, string version, string assetName, string destinationPath, CancellationToken cancellationToken)
//    {
//        var uri = $"https://github.com/{Organization}/{repository}/releases/download/v{version}/{assetName}";
//        var stream = await DownloadFile(uri, cancellationToken);
//        var path = Path.Combine(destinationPath, assetName);
//        WriteTo(stream, path);
//        return path;
//    }

//    public async Task<bool> ValidateDownloadedAsset(string path, string repository, string version, string assetName, CancellationToken cancellationToken)
//    {
//        var expectedHash = await DownloadHashAsset(repository, version, assetName, cancellationToken);
//        var data = File.ReadAllBytes(path);
//        var downloadHash = ComputeChecksum(data);

//        if (string.Equals(downloadHash.Trim(), expectedHash.Trim(), StringComparison.CurrentCultureIgnoreCase))
//            return true;

//        File.Delete(path);
//        return false;
//    }

//    public async Task<string> DownloadHashAsset(string repository, string version, string assetHashName, CancellationToken cancellationToken)
//    {
//        var uri = $"https://github.com/{Organization}/{repository}/releases/download/v{version}/{assetHashName}";
//        var stream = await DownloadFile(uri, cancellationToken);
//        return await GetAssetHashCode(stream, cancellationToken);
//    }

//    private async Task<string> GetAssetHashCode(Stream stream, CancellationToken cancellationToken)
//    {
//        using var sr = new StreamReader(stream);
//        var content = await sr.ReadToEndAsync(cancellationToken);
//        return content.Split('*')[0].Trim();
//    }

//    private async Task<Stream> DownloadFile(string uri, CancellationToken cancellationToken)
//    {
//        var client = new HttpClient();
//        var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
//        var response = await client.SendAsync(message, cancellationToken);

//        if (response.StatusCode == HttpStatusCode.NotFound)
//            throw new Exception($"Version not found from url: {uri}");

//        if (response.StatusCode != HttpStatusCode.OK)
//            throw new Exception($"Download failed with {response.StatusCode}");

//        return await response.Content.ReadAsStreamAsync(cancellationToken);
//    }

//    private string ComputeChecksum(byte[] data)
//    {
//        using (SHA256 sha256 = SHA256.Create())
//        {
//            byte[] hashBytes = sha256.ComputeHash(data);
//            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
//        }
//    }

//    private void WriteTo(Stream stream, string path)
//    {
//        using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
//        {
//            stream.CopyTo(fileStream);
//        }
//    }
//}

//internal class GitHubTag
//{
//    public required string Name { get; set; }
//}