using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Splat;
using Spore.CrossSeed;
using Spore.Gazelle;
using Spore.Login;
using Spore.Main;
using Spore.QBittorrent;

namespace Spore;

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

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables("SPORE_")
            .Build();
        RegisterClientConfiguration<RedactedClientConfiguration>(configuration);
        RegisterClientConfiguration<OrpheusClientConfiguration>(configuration);
        RegisterClientConfiguration<QBittorrentClientConfiguration>(configuration);

        var mainState = RxApp.SuspensionHost.GetAppState<MainState>();
        SplatRegistrations.RegisterConstant(mainState);

        SplatRegistrations.RegisterLazySingleton<MainViewModel>();
        SplatRegistrations.RegisterConstant<IScreen>(Locator.Current.GetRequiredService<MainViewModel>());

        SplatRegistrations.RegisterLazySingleton<LoginViewModel>();
        SplatRegistrations.RegisterLazySingleton<QBittorrentLoginViewModel>();
        SplatRegistrations.RegisterLazySingleton<QBittorrentClient>();
        SplatRegistrations.RegisterLazySingleton<LoginStatusBarViewModel>();

        SplatRegistrations.RegisterLazySingleton<CrossSeedViewModel>();

        SplatRegistrations.RegisterLazySingleton<RedactedClient>();
        SplatRegistrations.RegisterLazySingleton<RedactedLoginViewModel>();
        SplatRegistrations.Register<IViewFor<RedactedLoginViewModel>, GazelleLoginView>();

        SplatRegistrations.RegisterLazySingleton<OrpheusClient>();
        SplatRegistrations.RegisterLazySingleton<OrpheusLoginViewModel>();
        SplatRegistrations.Register<IViewFor<OrpheusLoginViewModel>, GazelleLoginView>();

        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());

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

    private static void RegisterClientConfiguration<T>(IConfiguration configuration)
        where T : IDefaultHttpHandlerConfiguration
    {
        SplatRegistrations.RegisterConstant(
            configuration
                .GetRequiredSection(typeof(T).Name)
                .Get<T>()!);
    }
}
