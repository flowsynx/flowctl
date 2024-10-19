using FlowCtl.Services.Abstracts;
using FlowSynx.Environment;
using FlowSynx.IO.Compression;

namespace FlowCtl.Services.Concretes;

internal class Extractor : IExtractor
{
    private readonly IOperatingSystemInfo _operatingSystemInfo;
    private readonly Func<CompressType, ICompression> _compressionFactory;

    public Extractor(IOperatingSystemInfo operatingSystemInfo, Func<CompressType, ICompression> compressionFactory)
    {
        _operatingSystemInfo = operatingSystemInfo;
        _compressionFactory = compressionFactory;
    }

    private string CompressionFileExtension => string.Equals(_operatingSystemInfo.Type, "windows", StringComparison.OrdinalIgnoreCase) ? "zip" : "tar.gz";

    public void ExtractFile(string sourcePath, string destinationPath)
    {
        var content = File.ReadAllBytes(sourcePath);
        var compressEntry = new CompressEntry
        {
            Content = content,
            Name = Path.GetFileName(sourcePath),
            ContentType = ""
        };

        if (CompressionFileExtension == "tar.gz")
            _compressionFactory(CompressType.GZip).Decompress(compressEntry, destinationPath);
        else
            _compressionFactory(CompressType.Zip).Decompress(compressEntry, destinationPath);
    }
}