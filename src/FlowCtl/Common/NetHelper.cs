using System.Net;

namespace FlowCtl.Common;

internal static class NetHelper
{
    public static async Task<Stream> DownloadFile(string uri, CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        var message = new HttpRequestMessage(HttpMethod.Get, new Uri(uri));
        var response = await client.SendAsync(message, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new Exception(string.Format(Resources.VersionNotFoundFromUrl, uri));

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception(string.Format(Resources.DownloadFailedWithStatus, response.StatusCode));

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
