using System.Runtime.InteropServices;

namespace FlowSynx.Cli.Common;

internal static class PathHelper
{
    public static string UserProfilePath => System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
    public static string DefaultFlowSynxDirectoryName => Path.Combine(UserProfilePath, ".flowsynx");
    public static string DefaultFlowSynxBinaryDirectoryName => Path.Combine(DefaultFlowSynxDirectoryName, "bin");

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

    public static string LookupDashboardBinaryFilePath(string path)
    {
        var binFileName = "dashboard";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return Path.Combine(path, binFileName);
    }

    public static string GetUpdateFilePath()
    {
        var binFileName = "Update";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            binFileName += ".exe";

        return binFileName;
    }

    public static string GetScriptFilePath()
    {
        var scriptFileName = "Update.sh";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            scriptFileName = "Update.bat";

        return scriptFileName;
    }

    public static void MakeExecutable(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        const UnixFileMode ownershipPermissions = UnixFileMode.UserRead  | UnixFileMode.UserWrite  | UnixFileMode.UserExecute  |
                                                  UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute |
                                                  UnixFileMode.OtherRead | UnixFileMode.OtherWrite | UnixFileMode.OtherExecute;

        File.SetUnixFileMode(path, ownershipPermissions);
    }

    public static void CopyFilesRecursively(string sourcePath, string targetPath, CancellationToken cancellationToken)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}