using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace Cultivator.Login;

public partial class QBittorrentLoginView : ReactiveUserControl<QBittorrentLoginViewModel>
{
    public QBittorrentLoginView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.QBittorrentClient.HostUrl, v => v.HostUrl.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.QBittorrentClient.Username, v => v.Username.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.QBittorrentClient.Password, v => v.Password.Text)
                .DisposeWith(disposables);

            var isNotAuthenticated = this
                .WhenAnyValue(v => v.ViewModel.QBittorrentClient.IsAuthenticated)
                .SelectMany(auth => auth)
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

            this.BindCommand(ViewModel, vm => vm.QBittorrentClient.LoginCommand, v => v.LoginButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.QBittorrentClient.LogoutCommand, v => v.LogoutButton)
                .DisposeWith(disposables);

            // ReSharper disable once RedundantCast
            ((StackPanel)LoginForm)
                .Events()
                .KeyUp
                .Where(e => e.Key == Key.Enter)
                .Select(_ => Unit.Default)
                .InvokeCommand(ViewModel.QBittorrentClient.LoginCommand);
        });
    }
}
