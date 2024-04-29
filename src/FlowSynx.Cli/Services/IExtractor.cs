namespace FlowSynx.Cli.Services;

internal interface IExtractor
{
    void ExtractFile(string sourcePath, string destinationPath);
}
