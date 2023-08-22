using System;
using Cultivator.Main;
using ReactiveUI;

namespace Cultivator.Gazelle;

public class RedactedLoginViewModel : GazelleLoginViewModelBase
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public RedactedLoginViewModel(MainState state, RedactedClient client) : base(client, "Redacted")
    {
        ApiKey = state.RedactedApiKey;

        // TODO dispose subscriptions

        this.WhenAnyValue(vm => vm.ApiKey)
            .Subscribe(apiKey => state.RedactedApiKey = apiKey);
    }
}
