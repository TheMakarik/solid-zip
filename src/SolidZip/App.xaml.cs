namespace SolidZip;

public partial class App 
{
    private const string ConfigurationPath = "configuration";
    private const string JsonExtension = ".json";
    private const string ApplicationStartupTimeLogMessage = "Application startup time: {time} ms";
    private const string ApplicationExitLogMessage = "Application is being exited with exit code {code}";
    private static readonly string AppDataFolderPath = Environment.ExpandEnvironmentVariables("%APPDATA%\\solid-zip\\");
    
    private const string StartupEventName = "STARTUP";
    private const string ExitEventName = "EXIT";
    private const string AppDataContentCreatedEventName = "APPDATACONTENT_CREATED";

    private const double AfterLoadingConfigurationProgress = 15.0d;
    private const double AfterLoadingLoggerProgress = 24.0d;
    private const double AfterLoadingDependenciesProgress = 44.0d;
    private const double AfterLoadingAppDataProgress = 62.0d;
    private const double AfterLoadingLuaProgress = 91.0d;
    private const double AfterPreparingProgress = 100.0d;
    
    private IHost _app;
    private readonly Stopwatch _applicationStartupTimeTimer = Stopwatch.StartNew();
    private Task _loadingLuaScripts;
    private StartupWindow _startupWindow;
    private StrongTypedLocalizationManager _localization;
    private Task _loadExplorerUserControlTask;

    protected override async void OnStartup(StartupEventArgs e)
    {
        this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        await InitializeApplicationAsync();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        _app.Services.GetRequiredService<ILogger<App>>().LogDebug(ApplicationExitLogMessage, e.ApplicationExitCode);
        var extensionsRaiser = _app.Services.GetRequiredService<ILuaExtensionsRaiser>();
        base.OnExit(e);
    }

    private async Task InitializeApplicationAsync()
    {
        InitializeStartupWindow();
        await BuildApplicationAsync();
        await LoadAppDataAsync();
        await LoadLuaAsync();
        await _loadExplorerUserControlTask;
        ShowMainView();
        StopTimer();
    }

    private void InitializeStartupWindow()
    {
        _localization = new StrongTypedLocalizationManager();
        _startupWindow = new StartupWindow() { ProgressText = _localization.LoadConfiguration };
        _startupWindow.Show();
    }

    private async Task BuildApplicationAsync()
    {
        var builder = Host.CreateApplicationBuilder();

        await LoadConfigurationAsync(builder);
        SetProgress(AfterLoadingConfigurationProgress, _localization.LoadLogger);
        
        await SetupLoggerAsync(builder);
        SetProgress(AfterLoadingLoggerProgress, _localization.LoadDependencies);

        await ConfigureServicesAsync(builder);
        SetProgress(AfterLoadingDependenciesProgress, _localization.LoadAppData + AppDataFolderPath);
        
        InitializeLuaLoading();
        ConfigureDependencyInjection();
    }

    private async Task LoadAppDataAsync()
    {
        await _app.Services.GetRequiredService<IAppDataContentCreator>().CreateAsync();
        SetProgress(AfterLoadingAppDataProgress, _localization.LoadLua);
        InitializeExplorerLoading();
    }

    private async Task LoadLuaAsync()
    {
        var extensionsRaiser = _app.Services.GetRequiredService<ILuaExtensionsRaiser>();
        SetProgress(AfterLoadingLuaProgress, _localization.Preparing);
        await _loadingLuaScripts;
        
        extensionsRaiser.RaiseBackground(AppDataContentCreatedEventName);
        extensionsRaiser.RaiseBackground(StartupEventName);
    }

    private void ShowMainView()
    {
        var mainView = _app.Services.GetView<MainWindow>();
        MainWindow = mainView;
        _startupWindow.Close();
        mainView.Show();
        SetProgress(AfterPreparingProgress, _localization.Preparing);
    }

    private void StopTimer()
    {
        _app.Services.GetRequiredService<ILogger<App>>().LogInformation(ApplicationStartupTimeLogMessage, _applicationStartupTimeTimer.ElapsedMilliseconds);
        _applicationStartupTimeTimer.Stop();
    }

    private Task LoadConfigurationAsync(HostApplicationBuilder builder)
    {
        return Task.Run(() =>
        {
            Directory.EnumerateFiles(ConfigurationPath)
                .Where(file => Path.GetExtension(file) == JsonExtension)
                .ForEach(path => builder.Configuration.AddJsonFile(path, optional: true, reloadOnChange: false));
        });
    }

    private Task SetupLoggerAsync(HostApplicationBuilder builder)
    {
        return Task.Run(() =>
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
                
            builder.Logging
                .ClearProviders()
                .AddSerilog(Log.Logger, dispose: true);
        });
    }

    private Task ConfigureServicesAsync(HostApplicationBuilder builder)
    {
        return Task.Run(() =>
        {
            builder.Services
                .Configure<AppDataOptions>(builder.Configuration.GetSection(nameof(AppDataOptions)))
                .Configure<ExplorerOptions>(builder.Configuration.GetSection(nameof(ExplorerOptions)))
                .Configure<LuaConfiguration>(builder.Configuration.GetSection(nameof(LuaConfiguration)))
                .Configure<ArchiveOptions>(builder.Configuration.GetSection(nameof(ArchiveOptions)))
                .AddExplorer()
                .AddProxies()
                .AddJsonSerialization()
                .AddAppDataServices()
                .AddFactories()
                .AddIconExtractors()
                .AddLua()
                .AddArchiveReader<ZipArchiveReader>()
                .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
                .AddSingleton(_localization)
                .AddSingleton<ViewModelLocator>()
                .Bind<ListExplorerItemsView, ExplorerViewModel>(viewModelLifetime: ServiceLifetime.Transient, ExplorerElementsView.ListBox)
                .Bind<MainWindow, MainWindowViewModel>(viewModelLifetime: ServiceLifetime.Singleton)
                .AddSingleton(Current);
            
            _app = builder.Build();
        });
    }

    private void InitializeLuaLoading()
    {
        _loadingLuaScripts = _app.Services.GetRequiredService<ILuaExtensionsLoader>()
            .LoadExtensionsAsync();
    }

    private void ConfigureDependencyInjection()
    {
        Ioc.Default.ConfigureServices(_app.Services);
        _app.RunAsync();
    }

    private void InitializeExplorerLoading()
    {
        _loadExplorerUserControlTask = LoadExplorerControlAsync();
    }

    private Task LoadExplorerControlAsync()
    {
        return Task.Factory.StartNew(async () =>
        {
            using var scope = _app.Services.CreateScope();
            var view = await scope.ServiceProvider
                .GetRequiredService<IAppDataContentManager>()
                .GetExplorerElementsViewAsync();
            var viewModel = scope.ServiceProvider.GetRequiredService<MainWindowViewModel>();
            viewModel.ExplorerControl = _app.Services.GetView(view);
        }, TaskCreationOptions.LongRunning);
    }

    private void SetProgress(double progress, string progressText)
    {
        Dispatcher.Invoke(() =>
        {
            _startupWindow.Progress = progress;
            _startupWindow.ProgressText = progressText;
        });
    }
}