using Spore.Main;

namespace Spore.Gazelle;

public class RedactedClient : GazelleClientBase
{
    public RedactedClient(MainState mainState, GazelleHandlerFactory gazelleHandlerFactory)
        : base(
            "https://<tracker-url>",
            mainState,
            state => state.LoginState.RedactedApiKey,
            (state, apiKey) => state.LoginState.RedactedApiKey = apiKey,
            gazelleHandlerFactory)
    {
    }
}
