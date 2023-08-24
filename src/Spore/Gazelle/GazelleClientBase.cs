using System;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Refit;
using Spore.Main;

namespace Spore.Gazelle;

public class GazelleClientConfigurationBase : IDefaultHttpHandlerConfiguration
{
    public GazelleClientConfigurationBase(
        string apiUrl,
        string trackerUrl,
        string sourceFlag,
        RequestPoliciesConfiguration? requestPolicies)
    {
        ApiUrl = apiUrl;
        TrackerUrl = trackerUrl;
        SourceFlag = sourceFlag;
        RequestPolicies = requestPolicies ?? new RequestPoliciesConfiguration();
    }

    public string ApiUrl { get; }
    public string TrackerUrl { get; }
    public string SourceFlag { get; }
    public RequestPoliciesConfiguration RequestPolicies { get; }
}

public abstract class GazelleClientBase : ReactiveObject
{
    private readonly IGazelleApi _api;
    private readonly BehaviorSubject<bool> _isAuthenticated = new(false);

    protected GazelleClientBase(
        GazelleClientConfigurationBase clientConfiguration,
        MainState mainState,
        Func<MainState, string?> getStateApiKey,
        Action<MainState, string?> setStateApiKey)
    {
        ApiKey = getStateApiKey(mainState);

        this.WhenAnyValue(vm => vm.ApiKey)
            .Subscribe(apiKey =>
            {
                setStateApiKey(mainState, apiKey);
                _isAuthenticated.OnNext(false);
            });

        _api = RestService.For<IGazelleApi>(
            clientConfiguration.ApiUrl,
            new RefitSettings
            {
                HttpMessageHandlerFactory = () =>
                    new GazelleHandler(
                        mainState,
                        getStateApiKey,
                        new DefaultHttpHandler(clientConfiguration.RequestPolicies))
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
