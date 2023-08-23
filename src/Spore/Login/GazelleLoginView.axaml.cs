using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Spore.Login;

public partial class GazelleLoginView : ReactiveUserControl<GazelleLoginViewModelBase>
{
    public GazelleLoginView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .Do(vm => Title.Text = vm.Title)
                .Subscribe()
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.GazelleClient.ApiKey, v => v.ApiKey.Text)
                .DisposeWith(disposables);

            var isNotAuthenticated = this
                .WhenAnyObservable(v => v.ViewModel.GazelleClient.IsAuthenticated)
                .Select(auth => !auth);
            var isNotLoggingIn = this
                .WhenAnyObservable(v => v.ViewModel.GazelleClient.LoginCommand.IsExecuting)
                .Select(isLoggingIn => !isLoggingIn);
            var formEnabled = isNotAuthenticated
                .CombineLatest(isNotLoggingIn, (notAuth, notLoggingIn) => notAuth && notLoggingIn);
            formEnabled
                .BindTo(this, v => v.ApiKey.IsEnabled)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.GazelleClient.LoginCommand, v => v.LoginButton)
                .DisposeWith(disposables);
        });
    }
}
