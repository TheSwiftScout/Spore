using ReactiveUI;

namespace Cultivator.Login;

public class LoginViewModel : ViewModelBase, IRoutableViewModel
{
    public LoginViewModel(
        IScreen hostScreen,
        QBittorrentLoginViewModel qBittorrentLoginViewModel,
        RedactedLoginViewModel redactedLoginViewModel,
        OrpheusLoginViewModel orpheusLoginViewModel)
    {
        HostScreen = hostScreen;
        QBittorrentLoginViewModel = qBittorrentLoginViewModel;
        RedactedLoginViewModel = redactedLoginViewModel;
        OrpheusLoginViewModel = orpheusLoginViewModel;
    }

    public QBittorrentLoginViewModel QBittorrentLoginViewModel { get; }
    public RedactedLoginViewModel RedactedLoginViewModel { get; }
    public OrpheusLoginViewModel OrpheusLoginViewModel { get; }

    public string UrlPathSegment => nameof(LoginViewModel);
    public IScreen HostScreen { get; }
}
