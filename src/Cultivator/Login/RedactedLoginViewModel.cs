using System;
using Cultivator.Gazelle;
using Cultivator.Main;
using ReactiveUI;

namespace Cultivator.Login;

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
