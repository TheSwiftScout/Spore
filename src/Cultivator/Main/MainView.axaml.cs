using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Cultivator.Main;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.QBittorrentViewModel, v => v.QBittorrentViewHost.ViewModel)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.RedactedLoginViewModel, v => v.RedactedLoginHost.ViewModel)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.OrpheusLoginViewModel, v => v.OrpheusLoginHost.ViewModel)
                .DisposeWith(disposables);
        });
    }
}
