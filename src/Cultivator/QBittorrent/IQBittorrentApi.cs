using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace Cultivator.QBittorrent;

public interface IQBittorrentApi
{
    public HttpClient Client { get; }

    // Authentication

    [Post("/api/v2/auth/login")]
    public Task Login(
        [Header("Referer")] string referer,
        [Body(BodySerializationMethod.UrlEncoded)]
        Dictionary<string, object> data);

    [Post("/api/v2/auth/logout")]
    public Task Logout();

    // Application

    [Get("/api/v2/app/version")]
    public Task<string> GetApplicationVersion();

    [Get("/api/v2/app/webapiVersion")]
    public Task<string> GetApiVersion();
}
