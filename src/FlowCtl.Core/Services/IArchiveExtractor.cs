namespace FlowCtl.Core.Services;

public interface IArchiveExtractor
{
    void ExtractArchive(string archivePath, string extractToFolder);
}
