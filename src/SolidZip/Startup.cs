
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
            .AddAppData()
            .AddPathsUtils()
            .AddExplorer()
            .AddSingleton<ApplicationViewsLoader>()
            .AddArchiving()
            .AddWpfConverter<ExplorerHistoryButtonForegroundConvertor>()
            .AddWpfConverter<PathToNameConvertor>()
            .AddWpfConverter<ExpandEnvironmentVariablesConverter>()
            .AddWpfConverter<PathToImageSourceConvertor>()
            .AddWpfConverter<BooleanToVisibilityConverter>()
            .AddWpfMultiConverter<NotNullImageSourceMultiValueConverter>()
            .AddCache<UserData>(async (data) =>
            {
                await Ioc.Default.GetRequiredService<RetrySystem>()
                    .RetryWithDelayAsync<InvalidOperationException>(new(async () =>
                    {
                        await using var stream = new FileStream(
                            Ioc.Default.GetRequiredService<PathsCollection>().UserData,
                            FileMode.Truncate);

                        await JsonSerializer.SerializeAsync(stream, data, UserDataSerializerContext.Default.Options);
                    }), maxRetry: 5, delay: TimeSpan.FromMilliseconds(100));

            });
        return hostBuilder.Build();
        
    }
    
}