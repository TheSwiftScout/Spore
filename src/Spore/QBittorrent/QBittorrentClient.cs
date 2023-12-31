﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

public class QBittorrentClientConfiguration : IDefaultHttpHandlerConfiguration
{
    public QBittorrentClientConfiguration(RequestPoliciesConfiguration? requestPolicies = null)
    {
        RequestPolicies = requestPolicies ?? new RequestPoliciesConfiguration();
    }

    public RequestPoliciesConfiguration RequestPolicies { get; }
}

public class QBittorrentClient : ReactiveObject
{
    private readonly BehaviorSubject<bool> _isAuthenticated = new(false);
    private readonly RefitSettings _refitSettings;

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public QBittorrentClient(QBittorrentClientConfiguration clientConfiguration, MainState mainState)
    {
        _refitSettings = new RefitSettings
        {
            HttpMessageHandlerFactory = () => new DefaultHttpHandler(clientConfiguration.RequestPolicies),
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
        GetTorrentListCommand = ReactiveCommand.CreateFromTask(GetTorrentList, IsAuthenticated);
        ExportCommand = ReactiveCommand.CreateFromTask(async (string hash) => await Export(hash));
    }

    [Reactive] private IQBittorrentApi? Api { get; set; }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    public ReactiveCommand<Unit, IReadOnlyCollection<QBittorrentTorrent>> GetTorrentListCommand { get; }

    public ReactiveCommand<string, Stream> ExportCommand { get; }

    public IObservable<bool> IsAuthenticated => _isAuthenticated.DistinctUntilChanged();

    [Reactive] public string? HostUrl { get; set; }

    [Reactive] public string? Username { get; set; }

    [Reactive] public string? Password { get; set; }

    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(HostUrl))
            throw new InvalidOperationException($"The {nameof(HostUrl)} must be set to be able to log in.");
        if (string.IsNullOrWhiteSpace(Username))
            throw new InvalidOperationException($"The {nameof(Username)} must be set to be able to log in.");
        if (string.IsNullOrWhiteSpace(Password))
            throw new InvalidOperationException($"The {nameof(Password)} must be set to be able to log in.");

        Api = RestService.For<IQBittorrentApi>(HostUrl, _refitSettings);

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

    private async Task<IReadOnlyCollection<QBittorrentTorrent>> GetTorrentList()
    {
        var torrentList = await Api.GetTorrentList();
        return torrentList.ToList();
    }

    public async Task<List<string>> GetTorrentPieceHashes(string hash)
    {
        var pieceHashes = await Api.GetTorrentPieceHashes(new Dictionary<string, object>
        {
            { "hash", hash }
        });

        return pieceHashes.ToList();
    }

    private async Task<Stream> Export(string hash)
    {
        var httpContent = await Api
            .Export(new Dictionary<string, object>
            {
                { "hash", hash }
            })
            .ConfigureAwait(false);

        await httpContent
            .LoadIntoBufferAsync()
            .ConfigureAwait(false);

        return await httpContent
            .ReadAsStreamAsync()
            .ConfigureAwait(false);
    }

    private static bool IsValidHttpUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
