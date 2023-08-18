namespace Cultivator.QBittorrent;

public class QBittorrentViewModelFactory
{
    private readonly QBittorrentClientFactory _qBittorrentClientFactory;

    public QBittorrentViewModelFactory(QBittorrentClientFactory qBittorrentClientFactory)
    {
        _qBittorrentClientFactory = qBittorrentClientFactory;
    }

    public QBittorrentViewModel Create(QBittorrentState qBittorrentState)
    {
        return new QBittorrentViewModel(qBittorrentState, _qBittorrentClientFactory);
    }
}
