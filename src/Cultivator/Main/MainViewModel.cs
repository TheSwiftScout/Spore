using System.Reactive;
using System.Reactive.Linq;
using Cultivator.Login;
using ReactiveUI;

namespace Cultivator.Main;

public class MainViewModel : ViewModelBase, IScreen
{
    private readonly ReactiveCommand<Unit, IRoutableViewModel> _openLoginCommand;

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor (DI)
    public MainViewModel(LoginViewModel loginViewModel, LoginStatusBarViewModel loginStatusBarViewModel)
    {
        LoginStatusBarViewModel = loginStatusBarViewModel;
        _openLoginCommand = ReactiveCommand.CreateFromObservable(() =>
            Router.NavigateAndReset.Execute(loginViewModel));

        this.WhenAnyValue(vm => vm._openLoginCommand)
            .Select(_ => Unit.Default)
            .InvokeCommand(_openLoginCommand);
    }

    public LoginStatusBarViewModel LoginStatusBarViewModel { get; }

    public RoutingState Router { get; } = new();
}
