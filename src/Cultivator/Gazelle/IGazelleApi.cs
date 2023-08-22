using System.Threading.Tasks;
using Refit;

namespace Cultivator.Gazelle;

public interface IGazelleApi
{
    [Post("/ajax.php?action=browse&searchstr={searchString}")]
    Task<GazelleResponse<object>> TorrentsSearch(string searchString);

    [Get("/ajax.php?action=torrent&hash={hash}")]
    Task<GazelleResponse<TorrentResponse>> TorrentDetails(string hash);
}

public record GazelleResponse<T>(string Status, T? Response, string Error)
{
    public bool IsSuccess => Status == "success";
};

public record TorrentResponse(TorrentGroup Group, Torrent Torrent);

public record TorrentGroup(int Id);

public record Torrent(int Id);
