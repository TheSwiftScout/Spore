namespace Cultivator.QBittorrent;

public record QBittorrentTorrent(
    string Name,
    string Hash,
    string SavePath,
    string State,
    string? Tracker);
