using System;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Spore.Main;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Refit;

namespace Spore.Gazelle;

public abstract class GazelleClientBase : ReactiveObject
{
    private readonly IGazelleApi _api;
    private readonly BehaviorSubject<bool> _isAuthenticated = new(false);

    protected GazelleClientBase(
        string trackerUrl,
        MainState mainState,
        Func<MainState, string?> getStateApiKey,
        Action<MainState, string?> setStateApiKey,
        GazelleHandlerFactory gazelleHandlerFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(trackerUrl, nameof(trackerUrl));

        ApiKey = getStateApiKey(mainState);

        this.WhenAnyValue(vm => vm.ApiKey)
            .Subscribe(apiKey =>
            {
                setStateApiKey(mainState, apiKey);
                _isAuthenticated.OnNext(false);
            });

        _api = RestService.For<IGazelleApi>(
            trackerUrl,
            new RefitSettings
            {
                HttpMessageHandlerFactory = () => gazelleHandlerFactory.Create(getStateApiKey)
            });

        var canLogin = this
            .WhenAnyValue(c => c.ApiKey)
            .Select(apiKey => !string.IsNullOrWhiteSpace(apiKey))
            .CombineLatest(IsAuthenticated.Select(auth => !auth))
            .Select(combined => combined is { First: true, Second: true })
            .DistinctUntilChanged();

        LoginCommand = ReactiveCommand.CreateFromTask(Login, canLogin);

        LoginCommand
            .ThrownExceptions
            .Subscribe(exception =>
            {
                if (exception is ApiException { StatusCode: HttpStatusCode.Unauthorized })
                    // TODO show error toast
                    return;

                RxApp.DefaultExceptionHandler.OnNext(exception);
            });
    }

    public IObservable<bool> IsAuthenticated => _isAuthenticated.DistinctUntilChanged();

    [Reactive] public string? ApiKey { get; set; }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    private async Task Login()
    {
        var response = await _api.TorrentsSearch(string.Empty);
        _isAuthenticated.OnNext(response.IsSuccess);
    }

    private async Task SearchTorrent(string searchString)
    {
        var response = await _api.TorrentsSearch(searchString);
    }
}
