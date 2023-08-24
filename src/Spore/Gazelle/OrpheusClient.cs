using Spore.Main;

namespace Spore.Gazelle;

public class OrpheusClientConfiguration : GazelleClientConfigurationBase
{
    public OrpheusClientConfiguration(
        string apiUrl,
        string trackerUrl,
        string sourceFlag,
        RequestPoliciesConfiguration? requestPolicies = null) : base(apiUrl, trackerUrl, sourceFlag, requestPolicies)
    {
    }
}

public class OrpheusClient : GazelleClientBase
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public OrpheusClient(OrpheusClientConfiguration clientConfiguration, MainState mainState)
        : base(
            clientConfiguration,
            mainState,
            state => state.LoginState.OrpheusApiKey,
            (state, apiKey) => state.LoginState.OrpheusApiKey = apiKey)
    {
    }
}
