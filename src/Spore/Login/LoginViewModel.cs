using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace Spore.Login;

public class LoginViewModel : RoutableViewModelBase
{
    public LoginViewModel(
        IScreen hostScreen,
        QBittorrentLoginViewModel qBittorrentLoginViewModel,
        RedactedLoginViewModel redactedLoginViewModel,
        OrpheusLoginViewModel orpheusLoginViewModel) : base(hostScreen)
    {
        QBittorrentLoginViewModel = qBittorrentLoginViewModel;
        RedactedLoginViewModel = redactedLoginViewModel;
        OrpheusLoginViewModel = orpheusLoginViewModel;

        IsFullyAuthenticated = this
            .WhenAnyObservable(
                vm => vm.QBittorrentLoginViewModel.QBittorrentClient.IsAuthenticated,
                vm => vm.RedactedLoginViewModel.GazelleClient.IsAuthenticated,
                vm => vm.OrpheusLoginViewModel.GazelleClient.IsAuthenticated,
                (qBittorrentAuth, redactedAuth, orpheusAuth) => qBittorrentAuth && redactedAuth && orpheusAuth)
            .DistinctUntilChanged();
    }

    public IObservable<bool> IsFullyAuthenticated { get; }

    public QBittorrentLoginViewModel QBittorrentLoginViewModel { get; }
    public RedactedLoginViewModel RedactedLoginViewModel { get; }
    public OrpheusLoginViewModel OrpheusLoginViewModel { get; }

    public override string Title => "Login";

    public override string UrlPathSegment => nameof(LoginViewModel);
}
