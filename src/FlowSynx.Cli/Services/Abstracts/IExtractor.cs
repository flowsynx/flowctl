namespace FlowCtl.Services.Abstracts;

internal interface IExtractor
{
    void ExtractFile(string sourcePath, string destinationPath);
}
