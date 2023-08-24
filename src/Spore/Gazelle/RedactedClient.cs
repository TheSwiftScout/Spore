using Spore.Main;

namespace Spore.Gazelle;

public class RedactedClientConfiguration : GazelleClientConfigurationBase
{
    public RedactedClientConfiguration(
        string apiUrl,
        string trackerUrl,
        string sourceFlag,
        RequestPoliciesConfiguration? requestPolicies = null) : base(apiUrl, trackerUrl, sourceFlag, requestPolicies)
    {
    }
}

public class RedactedClient : GazelleClientBase
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public RedactedClient(RedactedClientConfiguration clientConfiguration, MainState mainState)
        : base(
            clientConfiguration,
            mainState,
            state => state.LoginState.RedactedApiKey,
            (state, apiKey) => state.LoginState.RedactedApiKey = apiKey)
    {
    }
}
