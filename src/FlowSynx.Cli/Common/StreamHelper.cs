namespace FlowSynx.Cli.Common;

internal static class StreamHelper
{
    public static void WriteStream(string path, Stream stream)
    {
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            stream.CopyTo(fileStream);
        }
    }

    public static void SaveStreamToFile(Stream stream, string path)
    {
        using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            stream.CopyTo(fileStream);
        }
    }
}