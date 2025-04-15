namespace FlowCtl.Services;

public interface IVersion
{
    string FlowCtlVersion { get; }
    string GetVersionFromPath(string path);
}