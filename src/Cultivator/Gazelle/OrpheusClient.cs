using Cultivator.Main;

namespace Cultivator.Gazelle;

public class OrpheusClient : GazelleClientBase
{
    public OrpheusClient(MainState mainState, GazelleHandlerFactory gazelleHandlerFactory)
        : base(
            "https://<tracker-url>",
            mainState,
            state => state.LoginState.OrpheusApiKey,
            (state, apiKey) => state.LoginState.OrpheusApiKey = apiKey,
            gazelleHandlerFactory)
    {
    }
}
