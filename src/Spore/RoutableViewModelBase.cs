using ReactiveUI;

namespace Spore;

public abstract class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
{
    protected RoutableViewModelBase(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }

    public abstract string? UrlPathSegment { get; }

    public IScreen HostScreen { get; }
}
