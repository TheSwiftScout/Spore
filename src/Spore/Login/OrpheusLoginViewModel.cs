using Spore.Gazelle;

namespace Spore.Login;

public class OrpheusLoginViewModel : GazelleLoginViewModelBase
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public OrpheusLoginViewModel(OrpheusClient orpheusClient) : base(orpheusClient)
    {
    }

    public override string Title => "Orpheus";
}
