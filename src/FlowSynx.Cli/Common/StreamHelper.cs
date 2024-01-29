namespace FlowSynx.Cli.Common;

internal static class StreamHelper
{
    public static async Task WriteStream(string path, Stream stream, CancellationToken cancellationToken)
    {
        var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream, cancellationToken);
        await fileStream.DisposeAsync();
    }

    public static void SaveStreamToFile(Stream stream, string path)
    {
        var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        stream.CopyTo(fileStream);
        fileStream.Dispose();
    }
}