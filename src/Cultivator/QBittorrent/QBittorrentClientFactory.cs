namespace Cultivator.QBittorrent;

public class QBittorrentClientFactory
{
    private readonly TransientHttpErrorHandler _httpHandler;

    public QBittorrentClientFactory(TransientHttpErrorHandler httpHandler)
    {
        _httpHandler = httpHandler;
    }

    public QBittorrentClient Create(string hostUrl)
    {
        return new QBittorrentClient(hostUrl, _httpHandler);
    }
}
