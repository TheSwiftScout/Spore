using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace Spore.CrossSeed;

public partial class CrossSeedView : ReactiveUserControl<CrossSeedViewModel>
{
    public CrossSeedView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .Do(vm => Title.Text = vm.Title)
                .Subscribe()
                .DisposeWith(disposables);
        });
    }
}
