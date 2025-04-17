using FlowCtl.Services.Location;
using System.Runtime.InteropServices;

namespace FlowCtl.UnitTests.Services.Location;

public class LocationServiceTests
{
    private readonly LocationService _locationService;

    public LocationServiceTests()
    {
        _locationService = new LocationService();
    }

    [Fact]
    public void RootLocation_Should_NotBeNullOrEmpty()
    {
        Assert.False(string.IsNullOrEmpty(_locationService.RootLocation));
        Assert.True(Directory.Exists(_locationService.RootLocation));
    }

    [Fact]
    public void UserProfilePath_Should_Return_ValidPath()
    {
        var path = _locationService.UserProfilePath;
        Assert.False(string.IsNullOrEmpty(path));
        Assert.True(Directory.Exists(path));
    }

    [Fact]
    public void DefaultFlowSynxDirectoryName_Should_EndWithFlowsynx()
    {
        var path = _locationService.DefaultFlowSynxDirectoryName;
        Assert.EndsWith(Path.Combine(".flowsynx"), path);
    }

    [Fact]
    public void DefaultFlowSynxBinaryDirectoryName_Should_EndWithBin()
    {
        var path = _locationService.DefaultFlowSynxBinaryDirectoryName;
        Assert.EndsWith(Path.Combine(".flowsynx", "bin"), path);
    }

    [Fact]
    public void FlowSynxBinaryName_Should_Return_CorrectName()
    {
        Assert.Equal("flowsynx", _locationService.FlowSynxBinaryName);
    }

    [Theory]
    [InlineData("flowsynx")]
    [InlineData("flowctl")]
    public void LookupBinaryFilePath_Should_AppendExe_OnWindows(string binaryName)
    {
        var testPath = "/some/path";
        var expectedBinary = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $"{binaryName}.exe"
            : binaryName;

        var result = binaryName switch
        {
            "flowsynx" => _locationService.LookupFlowSynxBinaryFilePath(testPath),
            "flowctl" => _locationService.LookupFlowCtlBinaryFilePath(testPath),
            _ => throw new ArgumentException("Invalid binary name.")
        };

        Assert.EndsWith(Path.Combine(testPath, expectedBinary), result);
    }
}