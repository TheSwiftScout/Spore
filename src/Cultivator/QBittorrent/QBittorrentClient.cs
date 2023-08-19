using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading.Tasks;
using Refit;

namespace Cultivator.QBittorrent;

public class QBittorrentClient
{
    private readonly IQBittorrentApi _api;
    private readonly string _hostUrl;
    private readonly BehaviorSubject<bool> _isAuthenticated = new(false);

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public QBittorrentClient(string hostUrl, TransientHttpErrorHandler transientHttpErrorHandler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hostUrl, nameof(hostUrl));
        _hostUrl = hostUrl;

        _api = RestService.For<IQBittorrentApi>(
            hostUrl,
            new RefitSettings
            {
                HttpMessageHandlerFactory = () => transientHttpErrorHandler,
                ContentSerializer = new SystemTextJsonContentSerializer(
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    })
            });
    }

    public IObservable<bool> IsAuthenticated => _isAuthenticated.DistinctUntilChanged();

    public async Task Login(string username, string password)
    {
        await _api.Login(_hostUrl, new Dictionary<string, object>
        {
            { "username", username },
            { "password", password }
        });

        _isAuthenticated.OnNext(true);
    }

    public async Task Logout()
    {
        try
        {
            await _api.Logout();
        }
        catch (ApiException apiException)
        {
            if (apiException.StatusCode != HttpStatusCode.Forbidden)
                throw;
        }

        _isAuthenticated.OnNext(false);
    }

    public async Task<List<QBittorrentTorrent>> GetTorrentList()
    {
        return await _api.GetTorrentList();
    }

    public async Task<List<string>> GetTorrentPieceHashes(string hash)
    {
        return await _api.GetTorrentPieceHashes(new Dictionary<string, object>
        {
            { "hash", hash }
        });
    }
}
