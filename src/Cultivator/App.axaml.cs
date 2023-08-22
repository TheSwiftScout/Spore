using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Cultivator.Gazelle;
using Cultivator.Login;
using Cultivator.Main;
using Cultivator.QBittorrent;
using ReactiveUI;
using Splat;

namespace Cultivator;

public class App : Application
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
        RxApp.SuspensionHost.CreateNewAppState = () => new MainState();
        RxApp.SuspensionHost.SetupDefaultSuspendResume(new AkavacheSuspensionDriver<MainState>());
        suspension.OnFrameworkInitializationCompleted();

        SplatRegistrations.SetupIOC();

        SplatRegistrations.Register<TransientHttpErrorHandler>();

        var mainState = RxApp.SuspensionHost.GetAppState<MainState>();
        SplatRegistrations.RegisterConstant(mainState);

        SplatRegistrations.RegisterLazySingleton<MainViewModel>();
        SplatRegistrations.RegisterConstant<IScreen>(Locator.Current.GetRequiredService<MainViewModel>());

        SplatRegistrations.RegisterLazySingleton<LoginViewModel>();
        SplatRegistrations.RegisterLazySingleton<QBittorrentLoginViewModel>();
        SplatRegistrations.RegisterLazySingleton<QBittorrentClient>();
        SplatRegistrations.RegisterLazySingleton<LoginStatusBarViewModel>();

        SplatRegistrations.Register<GazelleHandlerFactory>();
        SplatRegistrations.Register<GazelleHandler>();

        SplatRegistrations.RegisterLazySingleton<RedactedClient>();
        SplatRegistrations.RegisterLazySingleton<RedactedLoginViewModel>();
        SplatRegistrations.Register<IViewFor<RedactedLoginViewModel>, GazelleLoginView>();

        SplatRegistrations.RegisterLazySingleton<OrpheusClient>();
        SplatRegistrations.RegisterLazySingleton<OrpheusLoginViewModel>();
        SplatRegistrations.Register<IViewFor<OrpheusLoginViewModel>, GazelleLoginView>();

        var assembly = Assembly.GetAssembly(GetType()) ??
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
