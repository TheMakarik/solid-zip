namespace SolidZip;

public partial class App 
{
    private const string ConfigurationPath = "configuration";
    private const string JsonExtension = ".json";
    private const string ApplicationStartupTimeLogMessage = "Application startup time: {time} ms";
    private const string ApplicationExitLogMessage = "Application is being exited with exit code {code}";
    
    private readonly IHost _app;
    private readonly Stopwatch _applicationStartupTimeTimer = Stopwatch.StartNew();
  
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
            .AddExplorer()
            .AddProxies()
            .AddJsonSerialization()
            .AddAppDataServices()
            .AddFactories()
            .AddIconExtractors()
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
            .AddSingleton<StrongTypedLocalizationManager>()
            .AddSingleton<ViewModelLocator>()
            .Bind<ListExplorerItemsView, ExplorerViewModel>()
            .Bind<MainWindow, MainWindowViewModel>();
        
        builder.Logging
            .ClearProviders()
            .AddSerilog(Log.Logger, dispose: true);

        _app = builder.Build();
        Ioc.Default.ConfigureServices(_app.Services);
        _app.RunAsync();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _app.Services.GetRequiredService<IAppDataContentCreator>().CreateAsync();
        _app.Services.GetView<MainWindow>().Show();
        _app.Services.GetRequiredService<ILogger<App>>().LogInformation(ApplicationStartupTimeLogMessage, _applicationStartupTimeTimer.ElapsedMilliseconds);
        _applicationStartupTimeTimer.Stop();
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _app.Services.GetRequiredService<ILogger<App>>().LogDebug(ApplicationExitLogMessage, e.ApplicationExitCode);
        base.OnExit(e);
    }
}