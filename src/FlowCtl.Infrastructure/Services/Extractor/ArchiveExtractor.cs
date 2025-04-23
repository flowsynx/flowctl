using FlowCtl.Core.Services.Extractor;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace FlowCtl.Infrastructure.Services.Extractor;

public class ArchiveExtractor : IArchiveExtractor
{
    private readonly Dictionary<string, Action<string, string>> _extractors;

    public ArchiveExtractor()
    {
        _extractors = new Dictionary<string, Action<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            { ".zip", ExtractZip },
            { ".tar.gz", ExtractTarGz },
            { ".tgz", ExtractTarGz }
        };
    }

    public void ExtractArchive(string archivePath, string extractToFolder)
    {
        var extension = GetExtension(archivePath);

        if (_extractors.TryGetValue(extension, out var extractor))
        {
            extractor(archivePath, extractToFolder);
        }
        else
        {
            throw new NotSupportedException(string.Format(Resources.ArchiveExtractor_NotSupportedFormat, extension));
        }
    }

    private static string GetExtension(string path)
    {
        if (path.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase)) return ".tar.gz";
        if (path.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase)) return ".tgz";
        return Path.GetExtension(path);
    }

    private void ExtractZip(string archivePath, string extractToFolder)
    {
        Directory.CreateDirectory(extractToFolder);
        using var archive = ZipArchive.Open(archivePath);
        foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
        {
            entry.WriteToDirectory(extractToFolder, new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = true
            });
        }
    }

    private void ExtractTarGz(string archivePath, string extractToFolder)
    {
        Directory.CreateDirectory(extractToFolder);
        using var stream = File.OpenRead(archivePath);
        using var reader = ReaderFactory.Open(stream);
        while (reader.MoveToNextEntry())
        {
            if (!reader.Entry.IsDirectory)
            {
                reader.WriteEntryToDirectory(extractToFolder, new ExtractionOptions
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
            }
        }
    }
}