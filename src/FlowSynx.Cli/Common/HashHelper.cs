using System.Security.Cryptography;
using System.Text;

namespace FlowSynx.Cli.Common;

internal static class HashHelper
{
    public static string ComputeSha256Hash(string filePath)
    {
        var file = new FileStream(filePath, FileMode.Open);
        using SHA256 sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(file);
        file.Close();

        var builder = new StringBuilder();

        foreach (var t in bytes)
            builder.Append(t.ToString("x2"));

        return builder.ToString();
    }

    public static async Task<string> GetAssetHashCode(Stream stream, CancellationToken cancellationToken)
    {
        using var sr = new StreamReader(stream);
        var content = await sr.ReadToEndAsync(cancellationToken);
        return content.Split('*')[0].Trim();
    }
}