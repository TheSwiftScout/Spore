using System;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Refit;

namespace Cultivator.Gazelle;

public abstract class GazelleLoginViewModelBase : ViewModelBase
{
    protected GazelleLoginViewModelBase(GazelleClient client, string title)
    {
        Client = client;
        Title = title;

        var canLogin = this
            .WhenAnyValue(vm => vm.ApiKey)
            .Select(apiKey => !string.IsNullOrWhiteSpace(apiKey))
            .DistinctUntilChanged();

        LoginCommand = ReactiveCommand.CreateFromTask(
            async () => await Client.SearchTorrent(string.Empty),
            canLogin);

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

    public string Title { get; }

    [Reactive] public string? ApiKey { get; set; }

    public GazelleClient Client { get; }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
}
