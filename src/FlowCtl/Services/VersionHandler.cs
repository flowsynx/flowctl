using System.Diagnostics;
using System.Reflection;

namespace FlowCtl.Services;

public class VersionHandler : IVersion
{
    public string FlowCtlVersion
    {
        get
        {
            var attributes = Assembly
                .GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);

            return attributes.Length == 0
                ? string.Empty
                : ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
        }
    }

    public string GetVersionFromPath(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return string.Empty;

        var versionInfo = FileVersionInfo.GetVersionInfo(path);
        return versionInfo.FileVersion ?? string.Empty;
    }
}