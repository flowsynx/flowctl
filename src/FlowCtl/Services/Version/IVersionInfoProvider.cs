namespace FlowCtl.Services.Version;

public interface IVersionInfoProvider
{
    string GetFileVersion(string path);
}