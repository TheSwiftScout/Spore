using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Spore.Main;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Router, v => v.RouterHost.Router)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.LoginStatusBarViewModel, v => v.LoginStatusBarHost.ViewModel)
                .DisposeWith(disposables);
        });
    }
}
