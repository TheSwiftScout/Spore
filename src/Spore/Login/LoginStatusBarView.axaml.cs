using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Spore.Login;

public partial class LoginStatusBarView : ReactiveUserControl<LoginStatusBarViewModel>
{
    public LoginStatusBarView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyObservable(v => v.ViewModel.QBittorrentClient.IsAuthenticated)
                .Select(IsAuthenticatedToString)
                .BindTo(this, v => v.QBittorrentStatus.Text)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.RedactedClient.IsAuthenticated)
                .Select(IsAuthenticatedToString)
                .BindTo(this, v => v.RedactedStatus.Text)
                .DisposeWith(disposables);

            this.WhenAnyObservable(v => v.ViewModel.OrpheusClient.IsAuthenticated)
                .Select(IsAuthenticatedToString)
                .BindTo(this, v => v.OrpheusStatus.Text)
                .DisposeWith(disposables);
        });
    }

    private static string IsAuthenticatedToString(bool isAuthenticated)
    {
        return isAuthenticated ? "Connected" : "Disconnected";
    }
}
