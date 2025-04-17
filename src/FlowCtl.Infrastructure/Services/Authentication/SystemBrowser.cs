using Duende.IdentityModel.OidcClient.Browser;
using System.Net;
using System.Text;

namespace FlowCtl.Infrastructure.Services.Authentication;

public class SystemBrowser : IBrowser
{
    private readonly int _port;

    public SystemBrowser(int port)
    {
        _port = port;
    }

    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{_port}/");
        listener.Start();

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = options.StartUrl,
            UseShellExecute = true
        });

        var context = await listener.GetContextAsync();

        var response = context.Response;
        string html = "<h1>You may close this window.</h1>";
        var buffer = Encoding.UTF8.GetBytes(html);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();

        return new BrowserResult
        {
            Response = context.Request.Url.ToString(),
            ResultType = BrowserResultType.Success
        };
    }
}