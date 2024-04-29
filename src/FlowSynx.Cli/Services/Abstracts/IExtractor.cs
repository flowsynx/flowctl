namespace FlowSynx.Cli.Services.Abstracts;

internal interface IExtractor
{
    void ExtractFile(string sourcePath, string destinationPath);
}
