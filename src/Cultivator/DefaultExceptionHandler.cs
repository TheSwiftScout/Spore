using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using ReactiveUI;
using Splat;

namespace Cultivator;

internal sealed class DefaultExceptionHandler : IObserver<Exception>, IEnableLogger
{
    public void OnNext(Exception value)
    {
        if (Debugger.IsAttached) Debugger.Break();

        var ex = new UnhandledErrorException(
            "An object implementing IHandleObservableErrors (often a ReactiveCommand or ObservableAsPropertyHelper) has caused an error and nothing has subscribed to ThrownExceptions to handle it.",
            value);

        this.Log().Error(ex);

        RxApp.MainThreadScheduler.Schedule(() => throw ex);
    }

    public void OnError(Exception error)
    {
        if (Debugger.IsAttached) Debugger.Break();
        RxApp.MainThreadScheduler.Schedule(() => throw new NotImplementedException());
    }

    public void OnCompleted()
    {
        if (Debugger.IsAttached) Debugger.Break();
        RxApp.MainThreadScheduler.Schedule(() => throw new NotImplementedException());
    }
}
