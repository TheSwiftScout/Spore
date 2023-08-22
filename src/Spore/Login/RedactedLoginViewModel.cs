using Spore.Gazelle;

namespace Spore.Login;

public class RedactedLoginViewModel : GazelleLoginViewModelBase
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public RedactedLoginViewModel(RedactedClient redactedClient) : base("Redacted", redactedClient)
    {
    }
}
