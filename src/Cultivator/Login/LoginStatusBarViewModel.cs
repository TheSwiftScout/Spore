using Cultivator.QBittorrent;

namespace Cultivator.Login;

public class LoginStatusBarViewModel : ViewModelBase
{
    public LoginStatusBarViewModel(QBittorrentClient qBittorrentClient)
    {
        QBittorrentClient = qBittorrentClient;
    }

    public QBittorrentClient QBittorrentClient { get; }
}
