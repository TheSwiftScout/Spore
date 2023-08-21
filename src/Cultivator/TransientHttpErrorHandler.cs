using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace Cultivator;

public class TransientHttpErrorHandler : DelegatingHandler
{
    private readonly AsyncPolicy<HttpResponseMessage> _policy;

    public TransientHttpErrorHandler(HttpMessageHandler? innerHandler = null)
        : base(innerHandler ?? new HttpClientHandler())
    {
        var maxDelay = TimeSpan.FromSeconds(45);

        var delay = Backoff
            .DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 50)
            .Select(delay => TimeSpan.FromTicks(Math.Min(delay.Ticks, maxDelay.Ticks)));

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(delay);

        var circuitBreaker = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));

        _policy = Policy.WrapAsync(retryPolicy, circuitBreaker);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await _policy.ExecuteAsync(
            async innerCancellationToken => await base.SendAsync(request, innerCancellationToken),
            cancellationToken);
    }
}
