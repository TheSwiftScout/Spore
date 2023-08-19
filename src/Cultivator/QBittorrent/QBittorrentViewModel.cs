using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Cultivator.QBittorrent;

public class QBittorrentViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<QBittorrentTorrent> _torrents;

    public QBittorrentViewModel(QBittorrentState state, QBittorrentClientFactory qBittorrentClientFactory)
    {
        HostUrl = state.HostUrl;
        Username = state.Username;
        Password = state.Password;

        // TODO dispose some subscriptions

        this.WhenAnyValue(vm => vm.HostUrl)
            .Subscribe(hostUrl => state.HostUrl = hostUrl);

        this.WhenAnyValue(vm => vm.Username)
            .Subscribe(username => state.Username = username);

        this.WhenAnyValue(vm => vm.Password)
            .Subscribe(password => state.Password = password);

        var isAuthenticated = this
            .WhenAnyValue(vm => vm.QBittorrentClient)
            .SelectMany(client => client is null ? Observable.Return(false) : client.IsAuthenticated)
            .DistinctUntilChanged();

        isAuthenticated.ToPropertyEx(this, vm => vm.IsAuthenticated);

        var canLogin = this
            .WhenAnyValue(
                vm => vm.HostUrl,
                vm => vm.Username,
                vm => vm.Password,
                (hostUrl, username, password) =>
                    IsValidHttpUrl(hostUrl) &&
                    !string.IsNullOrWhiteSpace(username) &&
                    !string.IsNullOrWhiteSpace(password))
            .CombineLatest(isAuthenticated.Select(auth => !auth))
            .Select(combined => combined is { First: true, Second: true })
            .DistinctUntilChanged();

        LoginCommand = ReactiveCommand.CreateFromTask(
            async () =>
            {
                QBittorrentClient = qBittorrentClientFactory.Create(HostUrl!);
                await QBittorrentClient!.Login(Username!, Password!);
            },
            canLogin);

        LogoutCommand = ReactiveCommand.CreateFromTask(
            async () =>
            {
                await QBittorrentClient!.Logout();
                QBittorrentClient = null;
            },
            isAuthenticated);

        var torrentsSourceCache = new SourceCache<QBittorrentTorrent, string>(torrent => torrent.Hash);

        torrentsSourceCache
            .Connect()
            .Filter(torrent => !string.IsNullOrWhiteSpace(torrent.Tracker))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _torrents)
            .Subscribe();

        var loadTorrentsList = ReactiveCommand.CreateFromTask(async () =>
        {
            var torrents = await QBittorrentClient.GetTorrentList();
            torrentsSourceCache.AddOrUpdate(torrents);
            var removedTorrents = torrentsSourceCache.Keys.Except(torrents.Select(t => t.Hash));
            torrentsSourceCache.RemoveKeys(removedTorrents);
        });

        this.WhenAnyValue(vm => vm.IsAuthenticated)
            .Where(auth => auth)
            .Select(_ => Unit.Default)
            .InvokeCommand(loadTorrentsList);
    }

    public ReadOnlyObservableCollection<QBittorrentTorrent> Torrents => _torrents;

    [Reactive] private QBittorrentClient? QBittorrentClient { get; set; }

    [ObservableAsProperty] public bool IsAuthenticated { get; set; }

    [Reactive] public string? HostUrl { get; set; }

    [Reactive] public string? Username { get; set; }

    [Reactive] public string? Password { get; set; }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    private static bool IsValidHttpUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
