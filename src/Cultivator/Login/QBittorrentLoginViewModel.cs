using Cultivator.QBittorrent;

namespace Cultivator.Login;

public class QBittorrentLoginViewModel : ViewModelBase
{
    public QBittorrentLoginViewModel(QBittorrentClient qBittorrentClient)
    {
        QBittorrentClient = qBittorrentClient;
    }

    public QBittorrentClient QBittorrentClient { get; }
}
