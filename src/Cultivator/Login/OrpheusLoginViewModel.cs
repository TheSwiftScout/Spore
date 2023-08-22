using Cultivator.Gazelle;

namespace Cultivator.Login;

public class OrpheusLoginViewModel : GazelleLoginViewModelBase
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public OrpheusLoginViewModel(OrpheusClient orpheusClient) : base("Orpheus", orpheusClient)
    {
    }
}
