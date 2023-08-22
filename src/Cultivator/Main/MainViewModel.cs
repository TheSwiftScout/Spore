using Cultivator.Gazelle;
using Cultivator.QBittorrent;

namespace Cultivator.Main;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(
        MainState state,
        QBittorrentViewModelFactory qBittorrentViewModelFactory,
        RedactedLoginViewModel redactedLoginViewModel,
        OrpheusLoginViewModel orpheusLoginViewModel)
    {
        QBittorrentViewModel = qBittorrentViewModelFactory.Create(state.QBittorrentState);
        RedactedLoginViewModel = redactedLoginViewModel;
        OrpheusLoginViewModel = orpheusLoginViewModel;
    }

    public QBittorrentViewModel QBittorrentViewModel { get; }
    public RedactedLoginViewModel RedactedLoginViewModel { get; }
    public OrpheusLoginViewModel OrpheusLoginViewModel { get; }
}
