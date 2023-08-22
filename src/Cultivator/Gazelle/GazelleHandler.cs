using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cultivator.Main;
using Polly;

namespace Cultivator.Gazelle;

public class GazelleHandler : DelegatingHandler
{
    private readonly Func<MainState, string?> _getApiTokenFromState;
    private readonly MainState _state;
    private readonly AsyncPolicy _policy;

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public GazelleHandler(
        Func<MainState, string?> getApiTokenFromState,
        MainState state,
        TransientHttpErrorHandler transientHttpErrorHandler) : base(transientHttpErrorHandler)
    {
        _getApiTokenFromState = getApiTokenFromState;
        _state = state;
        // TODO make configurable
        _policy = Policy.RateLimitAsync(10, TimeSpan.FromSeconds(10));
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var apiToken = _getApiTokenFromState(_state);

        if (string.IsNullOrWhiteSpace(apiToken))
            throw new InvalidOperationException("Gazelle API Token is required");
        request.Headers.Add("Authorization", apiToken);

        return await _policy.ExecuteAsync(
            async innerCancellationToken => await base.SendAsync(request, innerCancellationToken),
            cancellationToken);
    }
}
