using Spore.QBittorrent;

namespace Spore.Login;

public class QBittorrentLoginViewModel : ViewModelBase
{
    public QBittorrentLoginViewModel(QBittorrentClient qBittorrentClient)
    {
        QBittorrentClient = qBittorrentClient;
    }

    public QBittorrentClient QBittorrentClient { get; }
}
