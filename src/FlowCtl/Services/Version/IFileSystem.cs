namespace FlowCtl.Services.Version;

public interface IFileSystem
{
    bool FileExists(string path);
}