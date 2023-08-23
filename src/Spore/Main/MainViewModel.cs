using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Spore.CrossSeed;
using Spore.Login;

namespace Spore.Main;

public class MainViewModel : ViewModelBase, IScreen
{
    private readonly IObservable<bool> _isFullyAuthenticated;

    public MainViewModel(
        // ReSharper disable SuggestBaseTypeForParameterInConstructor (DI)
        LoginViewModel loginViewModel,
        CrossSeedViewModel crossSeedViewModel,
        // ReSharper restore SuggestBaseTypeForParameterInConstructor
        LoginStatusBarViewModel loginStatusBarViewModel)
    {
        Routes = new List<RoutableViewModelBase>
        {
            loginViewModel,
            crossSeedViewModel
        };

        LoginStatusBarViewModel = loginStatusBarViewModel;

        _isFullyAuthenticated = loginViewModel.IsFullyAuthenticated;

        // TODO dispose subscription
        this.WhenAnyValue(vm => vm.SelectedRoute)
            .WhereNotNull()
            .InvokeCommand<IRoutableViewModel, RoutingState>(Router, router => router.Navigate);

        this.WhenAnyObservable(vm => vm._isFullyAuthenticated)
            .Subscribe(isFullyAuthenticated =>
            {
                if (isFullyAuthenticated)
                {
                    // skip login view
                    SelectedRoute = Routes.First(route => route != loginViewModel);
                    return;
                }

                SelectedRoute = loginViewModel;
            });
    }

    public IReadOnlyCollection<RoutableViewModelBase> Routes { get; }

    [Reactive] public RoutableViewModelBase? SelectedRoute { get; set; }

    public LoginStatusBarViewModel LoginStatusBarViewModel { get; }

    public override string Title => "Spore";

    public RoutingState Router { get; } = new();
}
