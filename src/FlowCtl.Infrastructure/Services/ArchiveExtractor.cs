using FlowCtl.Core.Services;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace FlowCtl.Infrastructure.Services;

internal class ArchiveExtractor : IArchiveExtractor
{
    public void ExtractArchive(string archivePath, string extractToFolder)
    {
        if (archivePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            ExtractZip(archivePath, extractToFolder);
        }
        else if (archivePath.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase) || archivePath.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
        {
            ExtractTarGz(archivePath, extractToFolder);
        }
        else
        {
            throw new NotSupportedException("Unsupported archive format.");
        }
    }

    private void ExtractZip(string archivePath, string extractToFolder)
    {
        Directory.CreateDirectory(extractToFolder);
        using (var archive = ZipArchive.Open(archivePath))
        {
            foreach (var entry in archive.Entries)
            {
                if (!entry.IsDirectory)
                {
                    entry.WriteToDirectory(extractToFolder, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }

    }

    private void ExtractTarGz(string archivePath, string extractToFolder)
    {
        Directory.CreateDirectory(extractToFolder);
        using (var stream = File.OpenRead(archivePath))
        using (var reader = ReaderFactory.Open(stream))
        {
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    reader.WriteEntryToDirectory(extractToFolder, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }
    }
}