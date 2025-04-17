using FlowCtl.Infrastructure.Services.Github;
using Moq;
using Moq.Protected;
using Octokit;
using System.Net;

namespace FlowCtl.Infrastructure.UnitTests.Services.Github;

public class GitHubReleaseManagerTests
{
    private readonly Mock<IReleasesClient> _mockReleasesClient = new();
    private readonly Mock<IRepositoriesClient> _mockRepositoriesClient = new();
    private readonly Mock<IGitHubClient> _mockGitHubClient = new();
    private readonly GitHubReleaseManager _releaseManager;
    private readonly HttpClient _mockHttpClient;

    public GitHubReleaseManagerTests()
    {
        // Setup GitHub client hierarchy
        _mockGitHubClient.Setup(x => x.Repository).Returns(_mockRepositoriesClient.Object);
        _mockRepositoriesClient.Setup(x => x.Release).Returns(_mockReleasesClient.Object);

        // Setup HTTP client mock
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("dummy content")
            });

        _mockHttpClient = new HttpClient(handlerMock.Object);

        // Create the manager
        _releaseManager = new GitHubReleaseManager(_mockGitHubClient.Object, _mockHttpClient);
    }

    [Fact]
    public async Task GetLatestVersion_ReturnsTagName()
    {
        var release = CreateFakeRelease("v1.0.0", Array.Empty<ReleaseAsset>());

        _mockReleasesClient
            .Setup(x => x.GetLatest("test-owner", "test-repo"))
            .ReturnsAsync(release);

        var version = await _releaseManager.GetLatestVersion("test-owner", "test-repo");

        Assert.Equal("v1.0.0", version);
    }

    [Fact]
    public async Task DownloadAsset_DownloadsFileCorrectly()
    {
        var tempPath = Path.GetTempFileName();
        File.Delete(tempPath);

        var asset = CreateFakeReleaseAsset("readme.me", "https://tests.flowsynx.io/readme.me");
        var release = CreateFakeRelease("v2.0.0", new[] { asset });

        _mockReleasesClient
            .Setup(x => x.GetLatest("test-owner", "test-repo"))
            .ReturnsAsync(release);

        var resultPath = await _releaseManager.DownloadAsset("test-owner", "test-repo", "readme.me", tempPath);

        Assert.True(File.Exists(resultPath));
        Assert.Equal("dummy content", await File.ReadAllTextAsync(resultPath));
    }

    [Fact]
    public void GetAssetHashCode_ReturnsCorrectHash()
    {
        var tempPath = Path.GetTempFileName();
        File.WriteAllText(tempPath, "Test content");

        var hash = _releaseManager.GetAssetHashCode(tempPath);

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.Equal(64, hash.Length);
    }

    [Fact]
    public void ValidateDownloadedAsset_ReturnsTrue_WhenHashesMatch()
    {
        var dataPath = Path.GetTempFileName();
        var hashPath = Path.GetTempFileName();

        File.WriteAllText(dataPath, "Some test data");

        var hash = _releaseManager.GetAssetHashCode(dataPath);
        File.WriteAllText(hashPath, $"{hash}  {Path.GetFileName(dataPath)}");

        var result = _releaseManager.ValidateDownloadedAsset(dataPath, hashPath);

        Assert.True(result);
    }

    private Release CreateFakeRelease(string tagName, ReleaseAsset[]? assets = null)
    {
        assets ??= Array.Empty<ReleaseAsset>();
        return new Release("", "", "", "", 1L, "", tagName, "", "", "", false, false,
                DateTimeOffset.Now, default!, default!, "", "", assets);
    }

    private ReleaseAsset CreateFakeReleaseAsset(string name, string downloadUrl)
    {
        return new ReleaseAsset("", 1, "", name, "", "", "", 0, 0,
            DateTimeOffset.Now, DateTimeOffset.Now, downloadUrl, default);
    }
}