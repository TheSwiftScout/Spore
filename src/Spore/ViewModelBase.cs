using ReactiveUI;

namespace Spore;

public abstract class ViewModelBase : ReactiveObject
{
    public abstract string Title { get; }
}
