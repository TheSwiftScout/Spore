using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Cultivator.QBittorrent;

public partial class QBittorrentView : ReactiveUserControl<QBittorrentViewModel>
{
    public QBittorrentView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.HostUrl, v => v.HostUrl.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Username, v => v.Username.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Password, v => v.Password.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.LoginButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.LogoutCommand, v => v.LogoutButton)
                .DisposeWith(disposables);
        });
    }
}
