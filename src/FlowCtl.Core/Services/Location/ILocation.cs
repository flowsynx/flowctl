namespace FlowCtl.Core.Services.Location;

public interface ILocation
{
    string RootLocation { get; }
    string UserProfilePath { get; }
    string DefaultFlowSynxDirectoryName { get; }
    string DefaultFlowSynxBinaryDirectoryName { get; }
    string FlowSynxBinaryName { get; }

    string LookupFlowCtlBinaryFilePath(string path);
    string LookupFlowSynxBinaryFilePath(string path);
}