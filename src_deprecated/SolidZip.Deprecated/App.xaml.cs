using SolidZip.Deprecated.Localization;
using SolidZip.Deprecated.ViewModels;
using SolidZip.Deprecated.Views;

namespace SolidZip.Deprecated;

public partial class App 
{
    private const string ConfigurationPath = "configuration";
    private const string JsonExtension = ".json";
    private const string ApplicationStartupTimeLogMessage = "Application startup time: {time} ms";
    private const string ApplicationExitLogMessage = "Application is being exited with exit code {code}";
    private static readonly string AppDataFolderPath = Environment.ExpandEnvironmentVariables("%APPDATA%\\solid-zip\\");
    
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

    protected override void OnExit(ExitEventArgs e)
    {
        _app.Services.GetRequiredService<ILogger<App>>().LogDebug(ApplicationExitLogMessage, e.ApplicationExitCode);
        var extensionsRaiser = _app.Services.GetRequiredService<ILuaExtensionsRaiser>();
        base.OnExit(e);
    }

    private async Task InitializeApplicationAsync()
    {
        InitializeStartupWindow();
        BuildApplication();
        await LoadAppDataAsync();
        await LoadLocalizationAsync();
        await LoadLuaAsync();
        await _loadExplorerUserControlTask;
        ShowMainView();
        StopTimer();
    }

    private async Task LoadLocalizationAsync()
    {
        using var scope = _app.Services.CreateScope();
        var culture = await scope.ServiceProvider
            .GetRequiredService<IAppDataContentManager>()
            .GetCurrentCultureAsync();
        
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        
        InitializeExplorerLoading();
    }

    private void InitializeStartupWindow()
    {
        _localization = new StrongTypedLocalizationManager();
        _startupWindow = new StartupWindow() { ProgressText = _localization.LoadConfiguration };
        _startupWindow.Show();
    }

    private void BuildApplication()
    {
        var builder = Host.CreateApplicationBuilder();

        LoadConfiguration(builder);
        SetProgress(AfterLoadingConfigurationProgress, _localization.LoadLogger);
        
        SetupLogger(builder);
        SetProgress(AfterLoadingLoggerProgress, _localization.LoadDependencies);

        ConfigureServices(builder);
        SetProgress(AfterLoadingDependenciesProgress, _localization.LoadAppData + AppDataFolderPath);
        
        InitializeLuaLoading();
        ConfigureDependencyInjection();
    }

    private async Task LoadAppDataAsync()
    {
        await _app.Services.GetRequiredService<IAppDataContentCreator>().CreateAsync();
        SetProgress(AfterLoadingAppDataProgress, _localization.LoadLua);
    }

    private async Task LoadLuaAsync()
    {
        SetProgress(AfterLoadingLuaProgress, _localization.Preparing);
        await _loadingLuaScripts;
        
    }

    private void ShowMainView()
    {
        var mainView = _app.Services.GetView<MainWindow>();
        MainWindow = mainView;
        _startupWindow.Close();
        mainView.Show();
        SetProgress(AfterPreparingProgress, _localization.Preparing);
    }

    private void InitializeExplorerLoading()
    {
        _loadExplorerUserControlTask = LoadExplorerControlAsync();
    }
    
    private void StopTimer()
    {
        _app.Services.GetRequiredService<ILogger<App>>().LogInformation(ApplicationStartupTimeLogMessage, _applicationStartupTimeTimer.ElapsedMilliseconds);
        _applicationStartupTimeTimer.Stop();
    }

    private void LoadConfiguration(HostApplicationBuilder builder)
    {
        Directory.EnumerateFiles(ConfigurationPath)
            .Where(file => Path.GetExtension(file) == JsonExtension)
            .ForEach(path => builder.Configuration.AddJsonFile(path, optional: true, reloadOnChange: false));
    }

    private void SetupLogger(HostApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
            
        builder.Logging
            .ClearProviders()
            .AddSerilog(Log.Logger, dispose: true);
    }

    private void ConfigureServices(HostApplicationBuilder builder)
    {
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
            .AddSingleton(_localization)
            .AddSingleton<ViewModelLocator>()
            .Bind<ListExplorerItemsView, ExplorerViewModel>(viewModelLifetime: ServiceLifetime.Transient, ExplorerElementsView.ListBox)
            .Bind<MainWindow, MainWindowViewModel>(viewModelLifetime: ServiceLifetime.Singleton)
            .AddSingleton(Current);
        
        _app = builder.Build();
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



    private Task LoadExplorerControlAsync()
    {
        return Task.Factory.StartNew(async () =>
        {
            using var scope = _app.Services.CreateScope();
            var view = await scope.ServiceProvider
                .GetRequiredService<IAppDataContentManager>()
                .GetExplorerElementsViewAsync();
            var viewModel = scope.ServiceProvider.GetRequiredService<MainWindowViewModel>();
            await Dispatcher.InvokeAsync(() => viewModel.ExplorerControl = _app.Services.GetView(view));

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