using System.Diagnostics;
using System.Reflection;
using FlowSynx.Environment;

namespace FlowSynx.Cli;

public class CliVersion : IVersion
{
    private readonly string? _rootLocation = Path.GetDirectoryName(System.AppContext.BaseDirectory);

    public string Version => GetApplicationVersion();

    #region MyRegion
    private string GetApplicationVersion()
    {
        Assembly? thisAssembly = null;
        try
        {
            thisAssembly = Assembly.GetEntryAssembly();
        }
        finally
        {
            thisAssembly ??= Assembly.GetExecutingAssembly();
        }

        if (thisAssembly == null)
            throw new Exception("Error in reading executable application.");


        var fullAssemblyName = thisAssembly.Location;
        var versionInfo = FileVersionInfo.GetVersionInfo(fullAssemblyName);
        return versionInfo.ProductVersion ?? "1.0.0.0";
    }
    #endregion
}
