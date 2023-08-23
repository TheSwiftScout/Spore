using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Refit;
using Spore.Main;

namespace Spore.QBittorrent;

public class QBittorrentClient : ReactiveObject
{
    private readonly BehaviorSubject<bool> _isAuthenticated = new(false);
    private readonly RefitSettings _refitSettings;

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public QBittorrentClient(MainState mainState, TransientHttpErrorHandler transientHttpErrorHandler)
    {
        _refitSettings = new RefitSettings
        {
            HttpMessageHandlerFactory = () => transientHttpErrorHandler,
            ContentSerializer = new SystemTextJsonContentSerializer(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                })
        };

        var loginState = mainState.LoginState;

        HostUrl = loginState.QBittorrentHostUrl;
        Username = loginState.QBittorrentUsername;
        Password = loginState.QBittorrentPassword;

        this.WhenAnyValue(c => c.HostUrl)
            .DistinctUntilChanged()
            .Subscribe(hostUrl => loginState.QBittorrentHostUrl = hostUrl);

        this.WhenAnyValue(c => c.Username)
            .DistinctUntilChanged()
            .Subscribe(username => loginState.QBittorrentUsername = username);

        this.WhenAnyValue(c => c.Password)
            .DistinctUntilChanged()
            .Subscribe(password => loginState.QBittorrentPassword = password);

        var canLogin = this
            .WhenAnyValue(
                vm => vm.HostUrl,
                vm => vm.Username,
                vm => vm.Password,
                (hostUrl, username, password) =>
                    IsValidHttpUrl(hostUrl) &&
                    !string.IsNullOrWhiteSpace(username) &&
                    !string.IsNullOrWhiteSpace(password))
            .CombineLatest(IsAuthenticated.Select(auth => !auth))
            .Select(combined => combined is { First: true, Second: true })
            .DistinctUntilChanged();

        LoginCommand = ReactiveCommand.CreateFromTask(Login, canLogin);
        LogoutCommand = ReactiveCommand.CreateFromTask(Logout, IsAuthenticated);
    }

    [Reactive] private IQBittorrentApi? Api { get; set; }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    public IObservable<bool> IsAuthenticated => _isAuthenticated.DistinctUntilChanged();

    [Reactive] public string? HostUrl { get; set; }

    [Reactive] public string? Username { get; set; }

    [Reactive] public string? Password { get; set; }

    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(HostUrl))
            throw new InvalidOperationException($"{nameof(HostUrl)} is not set");
        if (string.IsNullOrWhiteSpace(Username))
            throw new InvalidOperationException($"{nameof(Username)} is not set");
        if (string.IsNullOrWhiteSpace(Password))
            throw new InvalidOperationException($"{nameof(Password)} is not set");

        Api = RestService.For<IQBittorrentApi>(
            HostUrl,
            _refitSettings);

        await Api.Login(HostUrl, new Dictionary<string, object>
        {
            { "username", Username },
            { "password", Password }
        });

        _isAuthenticated.OnNext(true);
    }

    private async Task Logout()
    {
        try
        {
            if (Api is not null)
                await Api.Logout();
        }
        catch (ApiException apiException)
        {
            if (apiException.StatusCode == HttpStatusCode.Forbidden)
                // This can happen when logging out with an invalid session cookie
                return;

            throw;
        }

        _isAuthenticated.OnNext(false);
    }

    public async Task<List<QBittorrentTorrent>> GetTorrentList()
    {
        return await Api.GetTorrentList();
    }

    public async Task<List<string>> GetTorrentPieceHashes(string hash)
    {
        return await Api.GetTorrentPieceHashes(new Dictionary<string, object>
        {
            { "hash", hash }
        });
    }

    public async Task<Stream> Export(string hash)
    {
        var httpContent = await Api.Export(new Dictionary<string, object>
        {
            { "hash", hash }
        });
        await httpContent.LoadIntoBufferAsync();
        return await httpContent.ReadAsStreamAsync();
    }

    private static bool IsValidHttpUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
