namespace Cli.Core.Services;

public interface IHttpHandler
{
    Task<T> GetRequest<T>(string uri, CancellationToken cancellationToken);
    Task<TOut> PostRequest<TIn, TOut>(string uri, TIn content, CancellationToken cancellationToken);
}