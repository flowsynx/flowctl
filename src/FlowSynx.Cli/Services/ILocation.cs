namespace FlowSynx.Cli.Services;

public interface ILocation
{
    string RootLocation { get; }
    string UserProfilePath { get; }
    string DefaultFlowSynxDirectoryName { get; }
    string DefaultFlowSynxBinaryDirectoryName { get; }

    string LookupSynxBinaryFilePath(string path);
    string LookupFlowSynxBinaryFilePath(string path);
    string LookupDashboardBinaryFilePath(string path);
    string GetUpdateFilePath();
    string GetScriptFilePath();
}