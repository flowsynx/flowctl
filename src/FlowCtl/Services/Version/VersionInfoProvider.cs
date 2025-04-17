using System.Diagnostics;

namespace FlowCtl.Services.Version;

public class VersionInfoProvider : IVersionInfoProvider
{
    public string GetFileVersion(string path)
    {
        var versionInfo = FileVersionInfo.GetVersionInfo(path);
        return versionInfo.FileVersion ?? string.Empty;
    }
}