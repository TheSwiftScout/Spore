using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Spore.QBittorrent;

namespace Spore.Login;

public class QBittorrentLoginViewModel : ViewModelBase
{
    public QBittorrentLoginViewModel(QBittorrentClient qBittorrentClient)
    {
        QBittorrentClient = qBittorrentClient;

        this.WhenAnyValue(vm => vm.QBittorrentClient)
            .Select(_ => Unit.Default)
            .InvokeCommand(QBittorrentClient.LoginCommand);
    }

    public QBittorrentClient QBittorrentClient { get; }

    public override string Title => "qBittorrent";
}
