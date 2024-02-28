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
        var binFileName = "flowsynx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }

    public static void MakeExecutable(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        const UnixFileMode ownershipPermissions = UnixFileMode.UserRead  | UnixFileMode.UserWrite  | UnixFileMode.UserExecute  |
                                                  UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute |
                                                  UnixFileMode.OtherRead | UnixFileMode.OtherWrite | UnixFileMode.OtherExecute;

        File.SetUnixFileMode(path, ownershipPermissions);
    }
}