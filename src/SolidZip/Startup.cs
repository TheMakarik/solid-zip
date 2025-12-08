using SolidZip.Views;

namespace SolidZip;

public sealed class Startup
{
    public IHost BuildHost()
    {
        var hostBuilder = Host.CreateApplicationBuilder();

        hostBuilder.Configuration
            .AddJsonFile("appsettings.json");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(hostBuilder.Configuration)
            .CreateLogger();
        
        hostBuilder.Logging
            .SetMinimumLevel(LogLevel.Trace)
            .ClearProviders()
            .AddSerilog(Log.Logger, dispose: true);
        
        hostBuilder.Services
            .Configure<PathsOptions>(hostBuilder.Configuration)
            .Configure<DefaultOptions>(hostBuilder.Configuration)
            .Configure<LocalizationOptions>(hostBuilder.Configuration)
            .AddThemes((value, key) =>
                Application.Current.Resources[key] = (SolidColorBrush)new BrushConverter().ConvertFrom(value))
            .AddViewModelLocator()
            .AddSingleton<RetrySystem>()
            .AddKeyedSingleton<Window, MainView>(ApplicationViews.MainView)
            .AddKeyedTransient<Window, SettingsView>(ApplicationViews.Settings)
            .AddWin32()
            .AddSingleton<StrongTypedLocalizationManager>()
            .AddLua()
            .AddSingleton<MainViewModel>()
            .AddTransient<SettingsViewModel>()
            .AddAppData()
            .AddPathsUtils()
            .AddExplorer()
            .AddSingleton<ApplicationViewsLoader>()
            .AddArchiving()
            .AddWpfConverter<ExplorerHistoryButtonForegroundConvertor>()
            .AddWpfConverter<PathToNameConvertor>()
            .AddWpfConverter<PathToImageSourceConvertor>()
            .AddCache<UserData>();
        return hostBuilder.Build();
    }
    
}