
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
            .AddSerilog(Log.Logger, true);

        hostBuilder.Services
            .Configure<EncodingOptions>(hostBuilder.Configuration)
            .Configure<PathsOptions>(hostBuilder.Configuration)
            .Configure<DefaultOptions>(hostBuilder.Configuration)
            .Configure<TheMakariksOptions>(hostBuilder.Configuration)
            .Configure<ExplorerOptions>(hostBuilder.Configuration)
            .Configure<LocalizationOptions>(hostBuilder.Configuration)
            .AddThemes((value, key) =>
                Application.Current.Resources[key] = new BrushConverter().ConvertFrom(value))
            .AddViewModelLocator()
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
            .AddSingleton<RetrySystem>()
            .AddWin32()
            .AddSingleton<StrongTypedLocalizationManager>()
            .AddLua()
            .AddAppData()
            .AddStateMachines()
            .AddPathsUtils()
            .AddHostedService<LogCompressor>()
            .AddExplorer()
            .AddEncodingDetector()
            .AddDialogHelper(
                (views, remember) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var provider = Ioc.Default.GetRequiredService<IServiceProvider>();
                        var view = provider.GetRequiredKeyedService<Window>(views);

                        remember(views, view);
                        Ioc.Default.GetRequiredService<ILogger<IDialogHelper>>()
                            .LogInformation("Loaded view, {view}", view);

                        view.ShowDialog();
                    });
                },
                view => ((Window)view).Close())
            .AddMessageBox((message, caption, button, icon) =>
                (MessageBoxResultEnum)(byte)MessageBox.Show(message, caption, (MessageBoxButton)(byte)button, (MessageBoxImage)(byte)icon))
            .AddRequirePassword()
            .AddArchiving()
            .AddWpfConverter<ExplorerHistoryButtonForegroundConvertor>()
            .AddWpfConverter<PathToNameConvertor>()
            .AddWpfConverter<StringToFlagImageSourceConverter>()
            .AddWpfConverter<ExpandEnvironmentVariablesConverter>()
            .AddWpfConverter<PathToImageSourceConvertor>()
            .AddWpfConverter<BooleanToVisibilityConverter>()
            .AddWpfConverter<DirectoryCreationalLocalizationProblemConverter>()
            .AddWpfConverter<BooleanToVisibilityConverter>()
            .AddWpfMultiConverter<NotNullImageSourceMultiValueConverter>()
            .AddWpfConverter<FileCreationalLocalizationProblemConverter>()
            .AddWpfConverter<ShowOnlyIfInDirectoryConverter>()
            .AddWindow<MainView>(ApplicationViews.MainView)
            .AddWindow<SettingsView>(ApplicationViews.Settings)
            .AddWindow<ZipArchiveCreatorView>(ApplicationViews.NewZip)
            .AddWindow<ErrorView>(ApplicationViews.Error)
            .AddWindow<DirectoryCreationView>(ApplicationViews.CreateFolder)
            .AddWindow<FileCreationView>(ApplicationViews.CreateFile)
            .AddWindow<StartupView>(ApplicationViews.Startup)
            .AddWindow<RequirePasswordView>(ApplicationViews.RequirePassword)
            .AddCache<UserData>(data =>
            {
                using var stream = new FileStream(
                    Ioc.Default.GetRequiredService<PathsCollection>().UserData,
                    FileMode.Truncate);
                JsonSerializer.Serialize(stream, data, UserDataSerializerContext.Default.Options);
                Ioc.Default.GetRequiredService<ILogger<SharedCache<UserData>>>()
                    .LogInformation("Expanded userdata from cache was successful");
            });
        var host = hostBuilder.Build();
        var messenger = host.Services.GetRequiredService<IMessenger>();
        var requirePassword = (IRecipient<RequirePasswordReadyMessage>)host.Services.GetRequiredService<IRequirePassword>();
        messenger.Register(requirePassword);
        return host;
    }
}