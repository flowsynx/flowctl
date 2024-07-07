using FlowSynx.Environment;

namespace FlowCtl.Services.Abstracts;

internal interface IVersionHandler : IVersion
{
    string GetApplicationVersion(string path);
    bool CheckVersions(string latestVersion, string currentVersion);
    string Normalize(string? version);
}