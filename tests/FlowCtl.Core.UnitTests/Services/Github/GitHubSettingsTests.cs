using FlowCtl.Core.Services.Github;

namespace FlowCtl.Core.UnitTests.Services.Github;

public class GitHubSettingsTests
{
    [Fact]
    public void FlowSynxArchiveFileName_ShouldContainRepositoryNameAndExtension()
    {
        var fileName = GitHubSettings.FlowSynxArchiveFileName;
        Assert.Contains(GitHubSettings.FlowSynxRepository, fileName);
        Assert.Matches(@"\.(zip|tar\.gz)$", fileName);
    }

    [Fact]
    public void FlowSynxArchiveTemporaryFileName_ShouldContainRepositoryName_AndRandomGuid()
    {
        var fileName1 = GitHubSettings.FlowSynxArchiveTemporaryFileName;
        var fileName2 = GitHubSettings.FlowSynxArchiveTemporaryFileName;

        Assert.Contains(GitHubSettings.FlowSynxRepository, fileName1);
        Assert.NotEqual(fileName1, fileName2); // Should generate new names each time
    }

    [Fact]
    public void FlowSynxArchiveHashFileName_ShouldEndWithSha256()
    {
        var fileName = GitHubSettings.FlowSynxArchiveHashFileName;
        Assert.EndsWith(".sha256", fileName);
    }

    [Fact]
    public void FlowCtlArchiveFileName_ShouldContainCorrectRepoName()
    {
        var fileName = GitHubSettings.FlowCtlArchiveFileName;
        Assert.Contains(GitHubSettings.FlowCtlRepository, fileName);
    }

    [Fact]
    public void FlowCtlArchiveHashFileName_ShouldEndWithSha256()
    {
        var fileName = GitHubSettings.FlowCtlArchiveHashFileName;
        Assert.EndsWith(".sha256", fileName);
    }
}