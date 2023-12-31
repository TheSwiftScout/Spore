using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace Spore.Login;

public partial class QBittorrentLoginView : ReactiveUserControl<QBittorrentLoginViewModel>
{
    public QBittorrentLoginView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .Do(vm => Title.Text = vm.Title)
                .Subscribe()
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.QBittorrentClient.HostUrl, v => v.HostUrl.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.QBittorrentClient.Username, v => v.Username.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.QBittorrentClient.Password, v => v.Password.Text)
                .DisposeWith(disposables);

            var isNotAuthenticated = this
                .WhenAnyObservable(v => v.ViewModel.QBittorrentClient.IsAuthenticated)
                .Select(auth => !auth);
            var isNotLoggingIn = this
                .WhenAnyObservable(v => v.ViewModel.QBittorrentClient.LoginCommand.IsExecuting)
                .Select(isLoggingIn => !isLoggingIn);
            var formEnabled = isNotAuthenticated
                .CombineLatest(isNotLoggingIn, (notAuth, notLoggingIn) => notAuth && notLoggingIn);
            formEnabled
                .BindTo(this, v => v.HostUrl.IsEnabled)
                .DisposeWith(disposables);
            formEnabled
                .BindTo(this, v => v.Username.IsEnabled)
                .DisposeWith(disposables);
            formEnabled
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
                .InvokeCommand(ViewModel, vm => vm.QBittorrentClient.LoginCommand)
                .DisposeWith(disposables);
        });
    }
}
