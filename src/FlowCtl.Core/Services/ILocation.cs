namespace FlowCtl.Core.Services;

public interface ILocation
{
    string RootLocation { get; }
    string UserProfilePath { get; }
    string DefaultFlowSynxDirectoryName { get; }
    string DefaultFlowSynxBinaryDirectoryName { get; }
    string FlowSynxBinaryName { get; }

    string LookupFlowCtlBinaryFilePath(string path);
    string LookupFlowSynxBinaryFilePath(string path);
    string GetUpdateFilePath();
    string GetScriptFilePath();
}