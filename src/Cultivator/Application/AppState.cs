using System.Runtime.Serialization;
using Cultivator.QBittorrent;

namespace Cultivator.Application;

[DataContract]
public class AppState
{
    [DataMember] public QBittorrentState QBittorrentState { get; set; } = new();
}
