namespace Cultivator.QBittorrent;

public class QBittorrentClientFactory
{
    private readonly TransientHttpErrorHandler _transientHttpErrorHandler;

    public QBittorrentClientFactory(TransientHttpErrorHandler transientHttpErrorHandler)
    {
        _transientHttpErrorHandler = transientHttpErrorHandler;
    }

    public QBittorrentClient Create(string hostUrl)
    {
        return new QBittorrentClient(hostUrl, _transientHttpErrorHandler);
    }
}
