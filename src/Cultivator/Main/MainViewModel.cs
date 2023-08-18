using Cultivator.QBittorrent;

namespace Cultivator.Main;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(MainState state, QBittorrentViewModelFactory qBittorrentViewModelFactory)
    {
        QBittorrentViewModel = qBittorrentViewModelFactory.Create(state.QBittorrentState);
    }

    public QBittorrentViewModel QBittorrentViewModel { get; }
}
