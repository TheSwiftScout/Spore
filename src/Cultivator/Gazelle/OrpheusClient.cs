namespace Cultivator.Gazelle;

public class OrpheusClient : GazelleClient
{
    public OrpheusClient(GazelleHandlerFactory gazelleHandlerFactory)
        : base("https://<tracker-url>", gazelleHandlerFactory.Create(state => state.OrpheusApiKey))
    {
    }
}
