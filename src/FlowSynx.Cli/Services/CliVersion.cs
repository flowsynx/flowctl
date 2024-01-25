using System.Diagnostics;
using FlowSynx.Environment;

namespace FlowSynx.Cli.Services;

public class CliVersion : IVersion
{
    public string Version => GetApplicationVersion();

    #region Private function
    private string GetApplicationVersion()
    {
        var assemblyLocation = System.Environment.ProcessPath;

        if (string.IsNullOrEmpty(assemblyLocation))
            return "1.0.0";

        var versionInfo = FileVersionInfo.GetVersionInfo(assemblyLocation);
        return versionInfo.ProductVersion ?? "1.0.0";
    }
    #endregion
}