using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Refit;

namespace Cultivator.Gazelle;

public class GazelleClient
{
    private readonly IGazelleApi _api;
    private readonly BehaviorSubject<bool> _isAuthenticated = new(false);

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public GazelleClient(string trackerUrl, GazelleHandler gazelleHandler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(trackerUrl, nameof(trackerUrl));

        _api = RestService.For<IGazelleApi>(
            trackerUrl,
            new RefitSettings
            {
                HttpMessageHandlerFactory = () => gazelleHandler
            });
    }

    public IObservable<bool> IsAuthenticated => _isAuthenticated.DistinctUntilChanged();

    public async Task SearchTorrent(string searchString)
    {
        var response = await _api.TorrentsSearch(searchString);
        _isAuthenticated.OnNext(response.IsSuccess);
    }
}
