namespace Cultivator.Gazelle;

public class RedactedClient : GazelleClient
{
    public RedactedClient(GazelleHandlerFactory gazelleHandlerFactory)
        : base("https://<tracker-url>", gazelleHandlerFactory.Create(state => state.RedactedApiKey))
    {
    }
}
