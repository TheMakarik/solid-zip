using SolidZip.Services.LuaServices;
using SolidZip.Services.LuaServices.Abstraction;

namespace SolidZip;

public partial class App 
{
    private const string ConfigurationPath = "configuration";
    private const string JsonExtension = ".json";
    private const string ApplicationStartupTimeLogMessage = "Application startup time: {time} ms";
    private const string ApplicationExitLogMessage = "Application is being exited with exit code {code}";
    
    private const string StartupEventName = "STARTUP";
    private const string ExitEventName = "EXIT";
    private const string AppDataContentCreatedEventName = "APPDATACONTENT_CREATED";
    
    private readonly IHost _app;
    private readonly Stopwatch _applicationStartupTimeTimer = Stopwatch.StartNew();
    private readonly Task _loadingLuaScripts;
    private readonly Window _startWindow;
  
    public App()
    {

        var builder = Host.CreateApplicationBuilder();

        Directory.EnumerateFiles(ConfigurationPath)
            .Where(file => Path.GetExtension(file) == JsonExtension)
            .ForEach(path => builder.Configuration.AddJsonFile(path, optional: true, reloadOnChange: false));
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        
        builder.Services
            .Configure<AppDataOptions>(builder.Configuration.GetSection(nameof(AppDataOptions)))
            .Configure<ExplorerOptions>(builder.Configuration.GetSection(nameof(ExplorerOptions)))
            .Configure<LuaConfiguration>(builder.Configuration.GetSection(nameof(LuaConfiguration)))
            .AddExplorer()
            .AddProxies()
            .AddJsonSerialization()
            .AddAppDataServices()
            .AddFactories()
            .AddIconExtractors()
            .AddLua()
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
            .AddSingleton<StrongTypedLocalizationManager>()
            .AddSingleton<ViewModelLocator>()
            .Bind<ListExplorerItemsView, ExplorerViewModel>()
            .Bind<MainWindow, MainWindowViewModel>();
        
        builder.Logging
            .ClearProviders()
            .AddSerilog(Log.Logger, dispose: true)
            .SetMinimumLevel(LogLevel.Trace);

        //for lua extensions
        builder.Services
            .AddSingleton(Current);
        
        _app = builder.Build();
        
        _loadingLuaScripts = _app.Services.GetRequiredService<ILuaExtensionsLoader>()
            .LoadExtensionsAsync();
        
        Ioc.Default.ConfigureServices(_app.Services);
        _app.RunAsync();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _app.Services.GetRequiredService<IAppDataContentCreator>().CreateAsync();
        var extensionsRaiser = _app.Services.GetRequiredService<ILuaExtensionsRaiser>();
        await _loadingLuaScripts;
        
        //We will raise this extensions here to not wait extensions subscriber and create app-data content
        extensionsRaiser.RaiseBackground(AppDataContentCreatedEventName);
        extensionsRaiser.RaiseBackground(StartupEventName);
        
        _app.Services.GetView<MainWindow>().Show();
        _app.Services.GetRequiredService<ILogger<App>>().LogInformation(ApplicationStartupTimeLogMessage, _applicationStartupTimeTimer.ElapsedMilliseconds);
        _applicationStartupTimeTimer.Stop();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        _app.Services.GetRequiredService<ILogger<App>>().LogDebug(ApplicationExitLogMessage, e.ApplicationExitCode);
        var extensionsRaiser = _app.Services.GetRequiredService<ILuaExtensionsRaiser>();
        //await extensionsRaiser.RaiseAsync(ExitEventName);
        base.OnExit(e);
    }
}