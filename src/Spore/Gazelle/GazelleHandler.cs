using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Spore.Main;

namespace Spore.Gazelle;

public class GazelleHandler : DelegatingHandler
{
    private readonly Func<MainState, string?> _getStateApiKey;
    private readonly MainState _mainState;

    public GazelleHandler(
        MainState mainState,
        Func<MainState, string?> getStateApiKey,
        HttpMessageHandler innerHandler) : base(innerHandler)
    {
        _mainState = mainState;
        _getStateApiKey = getStateApiKey;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var apiKey = _getStateApiKey(_mainState);

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException(
                "Retrieving the API key from the app state failed. Log in to provide an API key.");

        request.Headers.Add("Authorization", apiKey);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
