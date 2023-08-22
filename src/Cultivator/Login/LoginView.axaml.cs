using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Cultivator.Login;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.QBittorrentLoginViewModel, v => v.QBittorrentLoginHost.ViewModel)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.RedactedLoginViewModel, v => v.RedactedLoginHost.ViewModel)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.OrpheusLoginViewModel, v => v.OrpheusLoginHost.ViewModel)
                .DisposeWith(disposables);
        });
    }
}
