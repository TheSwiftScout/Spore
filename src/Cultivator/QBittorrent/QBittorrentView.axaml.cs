using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace Cultivator.QBittorrent;

public partial class QBittorrentView : ReactiveUserControl<QBittorrentViewModel>
{
    private readonly FlatTreeDataGridSource<QBittorrentTorrent> _treeGridDataSource;

    public QBittorrentView()
    {
        InitializeComponent();

        _treeGridDataSource = new FlatTreeDataGridSource<QBittorrentTorrent>(Enumerable.Empty<QBittorrentTorrent>())
        {
            Columns =
            {
                new TextColumn<QBittorrentTorrent, string>(
                    "Name",
                    torrent => torrent.Name),

                new TextColumn<QBittorrentTorrent, string>(
                    "Hash",
                    torrent => torrent.Hash)
            }
        };

        TorrentsDataGrid.Source = _treeGridDataSource;

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.HostUrl, v => v.HostUrl.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.Username, v => v.Username.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.Password, v => v.Password.Text)
                .DisposeWith(disposables);

            var isNotAuthenticated = this
                .WhenAnyValue(v => v.ViewModel.IsAuthenticated)
                .Select(auth => !auth);
            isNotAuthenticated
                .BindTo(this, v => v.HostUrl.IsEnabled)
                .DisposeWith(disposables);
            isNotAuthenticated
                .BindTo(this, v => v.Username.IsEnabled)
                .DisposeWith(disposables);
            isNotAuthenticated
                .BindTo(this, v => v.Password.IsEnabled)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.LoginButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.LogoutCommand, v => v.LogoutButton)
                .DisposeWith(disposables);

            isNotAuthenticated
                .BindTo(this, v => v.LoginForm.IsVisible)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.IsAuthenticated, v => v.TorrentsList.IsVisible)
                .DisposeWith(disposables);

            // ReSharper disable once RedundantCast
            ((StackPanel)LoginForm)
                .Events()
                .KeyUp
                .Where(e => e.Key == Key.Enter)
                .Select(_ => Unit.Default)
                .InvokeCommand(ViewModel.LoginCommand);

            this.OneWayBind(ViewModel, vm => vm.Torrents, v => v._treeGridDataSource.Items)
                .DisposeWith(disposables);
        });
    }
}
