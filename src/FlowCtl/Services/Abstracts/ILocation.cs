namespace FlowCtl.Services.Abstracts;

public interface ILocation
{
    string RootLocation { get; }
    string UserProfilePath { get; }
    string DefaultFlowSynxDirectoryName { get; }
    string DefaultFlowSynxBinaryDirectoryName { get; }
    string FlowSynxBinaryName { get; }
    string DashboardBinaryName { get; }

    string LookupFlowCtlBinaryFilePath(string path);
    string LookupFlowSynxBinaryFilePath(string path);
    string LookupDashboardBinaryFilePath(string path);
    string GetUpdateFilePath();
    string GetScriptFilePath();
}