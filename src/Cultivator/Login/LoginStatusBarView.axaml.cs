using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Cultivator.Login;

public partial class LoginStatusBarView : ReactiveUserControl<LoginStatusBarViewModel>
{
    public LoginStatusBarView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyObservable(v => v.ViewModel.QBittorrentClient.IsAuthenticated)
                .Select(auth => auth ? "Connected" : "Disconnected")
                .BindTo(this, v => v.QBittorrentStatus.Text)
                .DisposeWith(disposables);
        });
    }
}
