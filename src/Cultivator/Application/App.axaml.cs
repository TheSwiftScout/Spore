using System;
using System.Reflection;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Cultivator.ViewModels;
using Cultivator.Views;
using ReactiveUI;
using Splat;

namespace Cultivator.Application;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        RxApp.DefaultExceptionHandler = new DefaultExceptionHandler();

        // OnFrameworkInitializationCompleted is called after Avalonia initialization,
        // so ApplicationLifetime is not null
        var suspension = new AutoSuspendHelper(ApplicationLifetime!);
        RxApp.SuspensionHost.CreateNewAppState = () => new AppState();
        RxApp.SuspensionHost.SetupDefaultSuspendResume(new AkavacheSuspensionDriver<AppState>());
        suspension.OnFrameworkInitializationCompleted();

        var appState = RxApp.SuspensionHost.GetAppState<AppState>();

        SplatRegistrations.SetupIOC();

        SplatRegistrations.RegisterConstant(appState);

        SplatRegistrations.RegisterLazySingleton<MainViewModel>();

        var assembly = Assembly.GetAssembly(GetType());
        if (assembly is null)
            throw new InvalidOperationException("Registry assembly must not be null");
        Locator.CurrentMutable.RegisterViewsForViewModels(assembly);

        var mainView = new MainView
        {
            DataContext = Locator.Current.GetRequiredService<MainViewModel>()
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
