using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Cultivator.Suspension;
using Cultivator.ViewModels;
using Cultivator.Views;
using ReactiveUI;

namespace Cultivator;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // OnFrameworkInitializationCompleted is called after Avalonia initialization,
        // so ApplicationLifetime is not null
        var suspension = new AutoSuspendHelper(ApplicationLifetime!);
        RxApp.SuspensionHost.CreateNewAppState = () => new MainViewModel();
        RxApp.SuspensionHost.SetupDefaultSuspendResume(new AkavacheSuspensionDriver<MainViewModel>());
        suspension.OnFrameworkInitializationCompleted();

        var mainView = new MainView
        {
            DataContext = RxApp.SuspensionHost.GetAppState<MainViewModel>()
        };

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow
                {
                    Content = mainView
                };
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = mainView;
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
