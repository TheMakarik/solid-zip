using BooleanToVisibilityConverter = System.Windows.Controls.BooleanToVisibilityConverter;

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
            .Configure<TheMakariksOptions>(hostBuilder.Configuration)
            .Configure<ExplorerOptions>(hostBuilder.Configuration)
            .Configure<LocalizationOptions>(hostBuilder.Configuration)
            .AddThemes((value, key) =>
                Application.Current.Resources[key] = (SolidColorBrush)new BrushConverter().ConvertFrom(value))
            .AddViewModelLocator()
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
            .AddSingleton<RetrySystem>()
            .AddKeyedSingleton<Window, MainView>(ApplicationViews.MainView)
            .AddKeyedTransient<Window, SettingsView>(ApplicationViews.Settings)
            .AddWin32()
            .AddSingleton<StrongTypedLocalizationManager>()
            .AddLua()
            .AddSingleton<MainViewModel>()
            .AddTransient<SettingsViewModel>()
            .AddTransient<ErrorViewModel>()
            .AddKeyedTransient<Window, ErrorView>(ApplicationViews.Error)
            .AddAppData()
            .AddPathsUtils()
            .AddExplorer()
            .AddSingleton<ApplicationViewsLoader>()
            .AddArchiving()
            .AddWpfConverter<ExplorerHistoryButtonForegroundConvertor>()
            .AddWpfConverter<PathToNameConvertor>()
            .AddWpfConverter<StringToFlagImageSourceConverter>()
            .AddWpfConverter<ExpandEnvironmentVariablesConverter>()
            .AddWpfConverter<PathToImageSourceConvertor>()
            .AddWpfConverter<BooleanToVisibilityConverter>()
            .AddWpfMultiConverter<NotNullImageSourceMultiValueConverter>()
            .AddCache<UserData>((data) =>
            {
                using var stream = new FileStream(
                                Ioc.Default.GetRequiredService<PathsCollection>().UserData,
                                FileMode.Truncate);
                JsonSerializer.Serialize(stream, data, UserDataSerializerContext.Default.Options);
                Ioc.Default.GetRequiredService<ILogger<SharedCache<UserData>>>().LogInformation("Expanded userdata to cache was successful");
            });
        return hostBuilder.Build();
    }
    
}