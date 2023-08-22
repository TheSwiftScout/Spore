using System;
using Cultivator.Main;
using ReactiveUI;

namespace Cultivator.Gazelle;

public class OrpheusLoginViewModel : GazelleLoginViewModelBase
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public OrpheusLoginViewModel(MainState state, OrpheusClient orpheusClient) : base(orpheusClient, "Orpheus")
    {
        ApiKey = state.OrpheusApiKey;

        // TODO dispose subscriptions

        this.WhenAnyValue(vm => vm.ApiKey)
            .Subscribe(apiKey => state.OrpheusApiKey = apiKey);
    }
}
