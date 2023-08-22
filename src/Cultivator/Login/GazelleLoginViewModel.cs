using System;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using Cultivator.Gazelle;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Refit;

namespace Cultivator.Login;

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

        TestCommand = ReactiveCommand.CreateFromTask(
            async () => await Client.SearchTorrent(string.Empty),
            canLogin);

        TestCommand
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

    public ReactiveCommand<Unit, Unit> TestCommand { get; }
}
