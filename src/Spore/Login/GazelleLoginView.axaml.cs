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

            this.BindCommand(ViewModel, vm => vm.GazelleClient.LoginCommand, v => v.TestButton)
                .DisposeWith(disposables);
        });
    }
}
