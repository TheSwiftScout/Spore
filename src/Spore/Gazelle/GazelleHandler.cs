using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Spore.Main;
using Polly;

namespace Spore.Gazelle;

public class GazelleHandler : DelegatingHandler
{
    private readonly Func<MainState, string?> _getStateApiKey;
    private readonly AsyncPolicy _policy;
    private readonly MainState _mainState;

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public GazelleHandler(
        MainState mainState,
        Func<MainState, string?> getStateApiKey,
        TransientHttpErrorHandler transientHttpErrorHandler) : base(transientHttpErrorHandler)
    {
        _mainState = mainState;
        _getStateApiKey = getStateApiKey;
        // TODO make configurable
        _policy = Policy.RateLimitAsync(10, TimeSpan.FromSeconds(10));
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var apiKey = _getStateApiKey(_mainState);

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Gazelle API Key is required");

        request.Headers.Add("Authorization", apiKey);

        return await _policy.ExecuteAsync(
            async innerCancellationToken => await base.SendAsync(request, innerCancellationToken),
            cancellationToken);
    }
}
