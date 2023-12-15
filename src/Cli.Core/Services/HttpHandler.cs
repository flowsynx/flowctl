using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Cli.Core.Exceptions;
using Cli.Core.Serialization;

namespace Cli.Core.Services;

public class HttpHandler : IHttpHandler
{
    private readonly HttpClient _httpClient;
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public HttpHandler(HttpClient httpClient, ISerializer serializer, IDeserializer deserializer)
    {
        _httpClient = httpClient;
        _serializer = serializer;
        _deserializer = deserializer;
    }

    public async Task<T> GetRequest<T>(string uri, CancellationToken cancellationToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync(cancellationToken);

            return _deserializer.Deserialize<T>(result);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpHandlerException("HttpRequestException when calling the API");
        }
        catch (TimeoutException ex)
        {
            throw new HttpHandlerException("TimeoutException during call to API");
        }
        catch (OperationCanceledException ex)
        {
            throw new HttpHandlerException("Task was canceled during call to API");
        }
        catch (Exception ex)
        {
            throw new HttpHandlerException($"Unhandled exception when calling the API. Message: {ex.Message}");
        }
    }

    public async Task<TOut> PostRequest<TIn, TOut>(string uri, TIn content, CancellationToken cancellationToken)
    {
        try
        {
            var request = new StringContent(_serializer.Serialize(content), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(uri, request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpHandlerException(response.StatusCode == HttpStatusCode.NotFound
                    ? "The repository was not found"
                    : "Did not receive 200 OK status code");
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = _deserializer.Deserialize<TOut>(responseJson);
            if (result == null)
            {
                throw new HttpHandlerException("Failed to deserialize response");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpHandlerException("HttpRequestException when calling the API");
        }
        catch (TimeoutException ex)
        {
            throw new HttpHandlerException("TimeoutException during call to API");
        }
        catch (OperationCanceledException ex)
        {
            throw new HttpHandlerException("Task was canceled during call to API");
        }
        catch (Exception ex)
        {
            throw new HttpHandlerException($"Unhandled exception when calling the API. Message: {ex.Message}");
        }
    }
}