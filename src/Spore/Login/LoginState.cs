using System.Runtime.Serialization;

namespace Spore.Login;

[DataContract]
public class LoginState
{
    [DataMember] public string? QBittorrentHostUrl { get; set; }

    [DataMember] public string? QBittorrentUsername { get; set; }

    [DataMember] public string? QBittorrentPassword { get; set; }

    [DataMember] public string? RedactedApiKey { get; set; }

    [DataMember] public string? OrpheusApiKey { get; set; }
}
