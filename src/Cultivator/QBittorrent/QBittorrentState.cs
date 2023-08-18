using System.Runtime.Serialization;

namespace Cultivator.QBittorrent;

[DataContract]
public class QBittorrentState
{
    [DataMember] public string? HostUrl { get; set; }

    [DataMember] public string? Username { get; set; }

    [DataMember] public string? Password { get; set; }
}
