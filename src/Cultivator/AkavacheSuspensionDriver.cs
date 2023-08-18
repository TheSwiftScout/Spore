using System;
using System.Reactive;
using Akavache;
using ReactiveUI;

namespace Cultivator;

internal sealed class AkavacheSuspensionDriver<TAppState> : ISuspensionDriver where TAppState : class
{
    private static readonly string AppStateKey = typeof(TAppState).Name;
    private readonly IBlobCache _blobCache;

    public AkavacheSuspensionDriver(string appName = "Cultivator")
    {
        BlobCache.ApplicationName = appName;
        // TODO use BlobCache.Secure for credentials
        _blobCache = BlobCache.UserAccount;
    }

    public IObservable<object> LoadState()
    {
        return _blobCache.GetObject<TAppState>(AppStateKey).WhereNotNull();
    }

    public IObservable<Unit> SaveState(object state)
    {
        return _blobCache.InsertObject(AppStateKey, (TAppState)state);
    }

    public IObservable<Unit> InvalidateState()
    {
        return _blobCache.InvalidateObject<TAppState>(AppStateKey);
    }
}
