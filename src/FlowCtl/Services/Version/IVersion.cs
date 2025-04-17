namespace FlowCtl.Services.Version;

public interface IVersion
{
    string FlowCtlVersion { get; }
    string GetVersionFromPath(string path);
}