using FlowCtl.Core.Services.Location;
using System.Runtime.InteropServices;

namespace FlowCtl.Services.Location;

public class LocationService : ILocation
{
    private readonly string _rootLocation;

    public LocationService()
    {
        _rootLocation = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) 
            ?? throw new DirectoryNotFoundException(Resources.LocationService_ErrorDuringGettingRootLocation);
    }

    public string RootLocation => _rootLocation;

    public string UserProfilePath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public string DefaultFlowSynxDirectoryName => Path.Combine(UserProfilePath, ".flowsynx");

    public string DefaultFlowSynxBinaryDirectoryName => Path.Combine(DefaultFlowSynxDirectoryName, "bin");

    public string FlowSynxBinaryName => "flowsynx";

    public string LookupFlowSynxBinaryFilePath(string path) =>
        Path.Combine(path, AppendExecutableExtension("flowsynx"));

    public string LookupFlowCtlBinaryFilePath(string path) =>
        Path.Combine(path, AppendExecutableExtension("flowctl"));

    private static string AppendExecutableExtension(string binaryName) =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"{binaryName}.exe" : binaryName;
}