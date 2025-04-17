using System.Runtime.InteropServices;

namespace FlowCtl.Core.Services.Github;

public static class GitHubSettings
{
    // Organization and Repository Names
    public const string Organization = "flowsynx";
    public const string FlowSynxRepository = "flowsynx";
    public const string FlowCtlRepository = "flowctl";

    // Archive and Hash File Extensions
    private static readonly string HashFileExtension = "sha256";
    private static readonly string CompressionFileExtension = GetCompressionExtension();

    // OS & Architecture Info
    private static readonly string OSType = GetOperatingSystemType();
    private static readonly string Architecture = RuntimeInformation.ProcessArchitecture.ToString();

    // Archive Names
    private static readonly string ArchiveName = $"{OSType}-{Architecture}.{CompressionFileExtension}";
    private static string ArchiveTemporaryName => $"{Guid.NewGuid()}.{CompressionFileExtension}";
    private static string ArchiveTemporaryHashName => $"{Guid.NewGuid()}.{CompressionFileExtension}";

    // FlowSynx Filenames
    public static string FlowSynxArchiveFileName => $"{FlowSynxRepository}-{ArchiveName.ToLower()}";
    public static string FlowSynxArchiveTemporaryFileName => $"{FlowSynxRepository}-{ArchiveTemporaryName.ToLower()}";
    public static string FlowSynxArchiveHashFileName => $"{FlowSynxRepository}-{ArchiveName.ToLower()}.{HashFileExtension}";
    public static string FlowSynxArchiveTemporaryHashFileName => $"{FlowSynxRepository}-{ArchiveTemporaryHashName.ToLower()}.{HashFileExtension}";

    // FlowCtl Filenames
    public static string FlowCtlArchiveFileName => $"{FlowCtlRepository}-{ArchiveName.ToLower()}";
    public static string FlowCtlArchiveHashFileName => $"{FlowCtlRepository}-{ArchiveName.ToLower()}.{HashFileExtension}";

    // Determine OS platform
    private static string GetOperatingSystemType()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "Linux";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            return "OSX";
        return "Windows";
    }

    // Determine compression extension based on OS
    private static string GetCompressionExtension()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "zip" : "tar.gz";
    }
}