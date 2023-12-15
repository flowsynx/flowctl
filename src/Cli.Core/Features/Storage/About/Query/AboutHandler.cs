using Cli.Core.Models;
using Cli.Core.Services;
using EnsureThat;
using MediatR;
using System;

namespace Cli.Core.Features.Storage.About.Query;

internal class AboutHandler : IRequestHandler<AboutRequest, Result<AboutResponse>>
{
    private readonly IHttpHandler _httpHandler;

    public AboutHandler(IHttpHandler httpHandler)
    {
        EnsureArg.IsNotNull(httpHandler, nameof(httpHandler));
        _httpHandler = httpHandler;
        _httpHandler = httpHandler;
    }

    public async Task<Result<AboutResponse>> Handle(AboutRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpHandler.PostRequest<AboutRequest, Result<AboutResponse>>(@"http://localhost:4401/storage/about", request, cancellationToken);

            if (response is not { Succeeded: true })
                return await Result<AboutResponse>.FailAsync(response.Messages);

            return await Result<AboutResponse>.SuccessAsync(response.Data);
        }
        catch (Exception ex)
        {
            return await Result<AboutResponse>.FailAsync(new List<string> { ex.Message });
        }
    }
}