using System.Runtime.Serialization;
using Cultivator.QBittorrent;

namespace Cultivator.Main;

[DataContract]
public class MainState
{
    [DataMember] public QBittorrentState QBittorrentState { get; set; } = new();

    [DataMember] public string? RedactedApiKey { get; set; }

    [DataMember] public string? OrpheusApiKey { get; set; }
}
