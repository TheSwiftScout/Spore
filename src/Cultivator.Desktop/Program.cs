using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Cultivator.Application;
using Serilog;
using Serilog.Events;
using Splat;
using Splat.Serilog;

namespace Cultivator.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Serilog

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                "log.txt",
                LogEventLevel.Information,
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Locator.CurrentMutable.UseSerilogFullLogger();

        try
        {
            // Startup
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "An unexpected error has occurred and the application will now exit.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
