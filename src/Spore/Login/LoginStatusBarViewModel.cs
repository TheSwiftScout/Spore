using ReactiveUI;
using Spore.Gazelle;
using Spore.QBittorrent;

namespace Spore.Login;

public class LoginStatusBarViewModel : ReactiveObject
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
