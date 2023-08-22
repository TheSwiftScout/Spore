using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Cultivator.Gazelle;

public partial class GazelleLoginView : ReactiveUserControl<GazelleLoginViewModelBase>
{
    public GazelleLoginView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Title, v => v.Title.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.ApiKey, v => v.ApiKey.Text)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.LoginButton)
                .DisposeWith(disposables);
        });
    }
}
