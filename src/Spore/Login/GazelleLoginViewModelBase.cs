using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Spore.Gazelle;

namespace Spore.Login;

public abstract class GazelleLoginViewModelBase : ViewModelBase
{
    protected GazelleLoginViewModelBase(GazelleClientBase gazelleClient)
    {
        GazelleClient = gazelleClient;

        this.WhenAnyValue(vm => vm.GazelleClient)
            .Select(_ => Unit.Default)
            .InvokeCommand(GazelleClient.LoginCommand);
    }

    public GazelleClientBase GazelleClient { get; }
}
