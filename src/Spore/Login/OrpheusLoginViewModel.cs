using Spore.Gazelle;

namespace Spore.Login;

public class OrpheusLoginViewModel : GazelleLoginViewModelBase
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public OrpheusLoginViewModel(OrpheusClient orpheusClient) : base("Orpheus", orpheusClient)
    {
    }
}
