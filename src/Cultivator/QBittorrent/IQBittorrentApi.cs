﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace Cultivator.QBittorrent;

public interface IQBittorrentApi
{
    // Authentication

    [Post("/api/v2/auth/login")]
    Task Login(
        [Header("Referer")] string referer,
        [Body(BodySerializationMethod.UrlEncoded)]
        Dictionary<string, object> data);

    [Post("/api/v2/auth/logout")]
    Task Logout();

    // Application

    [Get("/api/v2/app/version")]
    Task<string> GetApplicationVersion();

    [Get("/api/v2/app/webapiVersion")]
    Task<string> GetApiVersion();

    // Torrent Management

    [Get("/api/v2/torrents/info")]
    Task<List<QBittorrentTorrent>> GetTorrentList();

    [Post("/api/v2/torrents/pieceHashes")]
    Task<List<string>> GetTorrentPieceHashes(
        [Body(BodySerializationMethod.UrlEncoded)]
        Dictionary<string, object> data);
}
