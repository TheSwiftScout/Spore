using System;
using Spore.Main;

namespace Spore.Gazelle;

public class GazelleHandlerFactory
{
    private readonly MainState _mainState;
    private readonly TransientHttpErrorHandler _transientHttpErrorHandler;

    public GazelleHandlerFactory(MainState mainState, TransientHttpErrorHandler transientHttpErrorHandler)
    {
        _mainState = mainState;
        _transientHttpErrorHandler = transientHttpErrorHandler;
    }

    public GazelleHandler Create(Func<MainState, string?> getStateApiKey)
    {
        return new GazelleHandler(_mainState, getStateApiKey, _transientHttpErrorHandler);
    }
}
