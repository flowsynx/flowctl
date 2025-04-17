using FlowCtl.Services.Version;
using Moq;

namespace FlowCtl.UnitTests.Services.Version;

public class VersionHandlerTests
{
    private readonly Mock<IFileSystem> _fileSystemMock;
    private readonly Mock<IVersionInfoProvider> _versionInfoProviderMock;
    private readonly VersionHandler _versionHandler;

    public VersionHandlerTests()
    {
        _fileSystemMock = new Mock<IFileSystem>();
        _versionInfoProviderMock = new Mock<IVersionInfoProvider>();
        _versionHandler = new VersionHandler(_fileSystemMock.Object, _versionInfoProviderMock.Object);
    }

    [Fact]
    public void GetVersionFromPath_ShouldReturnEmpty_IfPathIsNull()
    {
        // Act
        var result = _versionHandler.GetVersionFromPath(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetVersionFromPath_ShouldReturnEmpty_IfFileDoesNotExist()
    {
        // Arrange
        _fileSystemMock.Setup(fs => fs.FileExists("fake.exe")).Returns(false);

        // Act
        var result = _versionHandler.GetVersionFromPath("fake.exe");

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetVersionFromPath_ShouldReturnVersion_IfFileExists()
    {
        // Arrange
        var path = "valid.exe";
        var expectedVersion = "1.2.3.4";
        _fileSystemMock.Setup(fs => fs.FileExists(path)).Returns(true);
        _versionInfoProviderMock.Setup(v => v.GetFileVersion(path)).Returns(expectedVersion);

        // Act
        var result = _versionHandler.GetVersionFromPath(path);

        // Assert
        Assert.Equal(expectedVersion, result);
    }
}