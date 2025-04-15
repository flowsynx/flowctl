using FlowCtl.Core.Services;
using System.Runtime.InteropServices;

namespace FlowCtl.Services;

public class Location : ILocation
{
    private readonly string? _rootLocation = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

    public Location()
    {
        if (_rootLocation == null)
        {
            throw new Exception(Resources.FlowSynxLocationBaseLocationNotFound);
        }
    }

    public string RootLocation => GetRootLocation();

    public string UserProfilePath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public string DefaultFlowSynxDirectoryName => Path.Combine(UserProfilePath, ".flowsynx");

    public string DefaultFlowSynxBinaryDirectoryName => Path.Combine(DefaultFlowSynxDirectoryName, "bin");

    public string FlowSynxBinaryName => "flowsynx";

    public string GetScriptFilePath()
    {
        var scriptFileName = "Update.sh";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            scriptFileName = "Update.bat";

        return scriptFileName;
    }

    public string GetUpdateFilePath()
    {
        var binFileName = "Update";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return binFileName;
    }

    public string LookupFlowSynxBinaryFilePath(string path)
    {
        var binFileName = "flowsynx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }

    public string LookupFlowCtlBinaryFilePath(string path)
    {
        var binFileName = "flowctl";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }

    #region MyRegion
    private string GetRootLocation()
    {
        if (_rootLocation is not null) return _rootLocation;
        throw new Exception(Resources.FlowSynxLocationRootLocationNotFound);
    }
    #endregion
}