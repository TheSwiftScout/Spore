using Cultivator.Gazelle;
using Cultivator.QBittorrent;

namespace Cultivator.Login;

public class LoginStatusBarViewModel : ViewModelBase
{
    public LoginStatusBarViewModel(
        QBittorrentClient qBittorrentClient,
        RedactedClient redactedClient,
        OrpheusClient orpheusClient)
    {
        QBittorrentClient = qBittorrentClient;
        RedactedClient = redactedClient;
        OrpheusClient = orpheusClient;
    }

    public QBittorrentClient QBittorrentClient { get; }
    public RedactedClient RedactedClient { get; }
    public OrpheusClient OrpheusClient { get; }
}
