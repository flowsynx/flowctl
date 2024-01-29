using System.Runtime.InteropServices;

namespace FlowSynx.Cli.Common;

internal static class PathHelper
{
    public static string UserProfilePath => System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
    public static string DefaultFlowSynxDirectoryName => Path.Combine(UserProfilePath, ".flowsynx");

    public static string LookupSynxBinaryFilePath(string path)
    {
        var binFileName = "synx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }

    public static string LookupFlowSynxBinaryFilePath(string path)
    {
        var binFileName = "FlowSynx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }
}