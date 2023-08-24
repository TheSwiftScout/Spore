using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace Spore;

public interface IDefaultHttpHandlerConfiguration
{
    RequestPoliciesConfiguration RequestPolicies { get; }
}

public class RequestPoliciesConfiguration
{
    public BulkheadConfiguration Bulkhead { get; set; } = new();
    public RateLimitConfiguration? RateLimit { get; set; }
}

public record RateLimitConfiguration(
    int NumberOfExecutions = 5,
    int PerTimeSpanSeconds = 10,
    int MaxBurst = 3);

public record BulkheadConfiguration(
    int MaxParallelization = 3,
    int MaxQueuingActions = 25000);

public class DefaultHttpHandler : DelegatingHandler
{
    private readonly AsyncPolicy<HttpResponseMessage> _policy;

    public DefaultHttpHandler(RequestPoliciesConfiguration? configuration) : base(new HttpClientHandler())
    {
        configuration ??= new RequestPoliciesConfiguration();

        var bulkhead = Policy.BulkheadAsync<HttpResponseMessage>(
            configuration.Bulkhead.MaxParallelization,
            configuration.Bulkhead.MaxQueuingActions);

        // TODO make configurable
        var maxDelay = TimeSpan.FromSeconds(45);
        var delay = Backoff
            .DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 50)
            .Select(delay => TimeSpan.FromTicks(Math.Min(delay.Ticks, maxDelay.Ticks)));
        var retry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(delay);

        // TODO make configurable
        var circuitBreaker = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));

        _policy = Policy.WrapAsync(bulkhead, retry, circuitBreaker);

        if (configuration.RateLimit is null) return;

        var rateLimit = Policy.RateLimitAsync<HttpResponseMessage>(
            configuration.RateLimit.NumberOfExecutions,
            TimeSpan.FromSeconds(configuration.RateLimit.PerTimeSpanSeconds),
            configuration.RateLimit.MaxBurst);

        _policy = rateLimit.WrapAsync(_policy);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await _policy.ExecuteAsync(
            async innerCancellationToken => await base.SendAsync(request, innerCancellationToken).ConfigureAwait(false),
            cancellationToken);
    }
}
