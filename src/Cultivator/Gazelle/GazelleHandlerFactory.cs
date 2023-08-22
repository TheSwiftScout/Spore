using System;
using Cultivator.Main;

namespace Cultivator.Gazelle;

public class GazelleHandlerFactory
{
    private readonly MainState _state;
    private readonly TransientHttpErrorHandler _transientHttpErrorHandler;

    public GazelleHandlerFactory(MainState state, TransientHttpErrorHandler transientHttpErrorHandler)
    {
        _state = state;
        _transientHttpErrorHandler = transientHttpErrorHandler;
    }

    public GazelleHandler Create(Func<MainState, string?> getApiTokenFromState)
    {
        return new GazelleHandler(getApiTokenFromState, _state, _transientHttpErrorHandler);
    }
}
