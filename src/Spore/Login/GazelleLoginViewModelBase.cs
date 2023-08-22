using Spore.Gazelle;

namespace Spore.Login;

public abstract class GazelleLoginViewModelBase : ViewModelBase
{
    protected GazelleLoginViewModelBase(string title, GazelleClientBase gazelleClient)
    {
        Title = title;
        GazelleClient = gazelleClient;
    }

    public string Title { get; }

    public GazelleClientBase GazelleClient { get; }
}
