using ReactiveUI;

namespace Spore.CrossSeed;

public class CrossSeedViewModel : RoutableViewModelBase
{
    public CrossSeedViewModel(IScreen hostScreen) : base(hostScreen)
    {
    }

    public override string Title => "Cross-seed";

    public override string UrlPathSegment => nameof(CrossSeedViewModel);
}
