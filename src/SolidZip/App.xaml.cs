namespace SolidZip;

public sealed partial class App
{
    private const double LuaExtensionsProgressBarPart = 60d;
    private const double AttachConsoleProgressBarPart = 20d;
    private const double LuaStartupPluginsBarPart = 30d;

    private readonly IHost _host = new Startup().BuildHost();
    private ILogger<App> _logger;
    private IMessenger _messenger;
    private Window _startup;

    public App()
    {
        RegisterExceptionView();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        Ioc.Default.ConfigureServices(_host.Services);
        _logger = _host.Services.GetRequiredService<ILogger<App>>();
        _messenger = Ioc.Default.GetRequiredService<IMessenger>();

        if (e.Args.Any())
            _logger.LogDebug("Startup args: {args}", e.Args);

        _startup = Ioc.Default.GetRequiredService<IServiceProvider>()
            .GetRequiredKeyedService<Window>(ApplicationViews.Startup);

        await LoadThemeAsync();
        MainWindow = _host.Services.GetKeyedService<Window>(ApplicationViews.MainView);

        var progress = new Progress<double>();
        progress.ProgressChanged += (_, args) => { _messenger.Send(new UpdateProgressMessage(args)); };

        var reportableIProgress = (IProgress<double>)progress;
        var task = LoadLuaPlugins(progress);
        _startup.Show();
        await LoadApplicationAsync();
        AttachLuaConsole();
        await task;
        reportableIProgress.Report(AttachConsoleProgressBarPart);
        await RaiseLuaEventsAsync();
        reportableIProgress.Report(LuaExtensionsProgressBarPart);
        _startup.Close();
        MainWindow?.Show();
        _host.StartAsync();
        base.OnStartup(e);
    }


    private async Task LoadThemeAsync()
    {
        await using var scope = _host.Services.CreateAsyncScope();

        var themeName = await scope
            .ServiceProvider
            .GetRequiredService<IUserJsonManager>()
            .GetThemeNameAsync();

        var themeLoader = scope.ServiceProvider.GetRequiredService<IThemeLoader>();
        await themeLoader.LoadAsync(themeName);
    }

    private async Task RaiseLuaEventsAsync()
    {
        var raiser = _host.Services.GetRequiredService<ILuaEventRaiser>();
        await raiser.RaiseAsync("init")
            .AsTask()
            .ContinueWith(async task => { await raiser.RaiseBackground("startup"); });
    }

    private Task LoadLuaPlugins(IProgress<double> progress)
    {
        return _host.Services.GetRequiredService<ILuaEventLoader>()
            .LoadAsync(progress, LuaExtensionsProgressBarPart);
    }

    private void AttachLuaConsole()
    {
        var console = _host.Services.GetRequiredService<ILuaDebugConsole>();
        console.AttachAsync();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        var raiser = _host.Services.GetRequiredService<ILuaEventRaiser>();
        var task = raiser.RaiseAsync("exit", e.ApplicationExitCode);
        ExpandUserData();
        await task;
        base.OnExit(e);
    }

    private void ExpandUserData()
    {
        _host.Services
            .GetRequiredService<SharedCache<UserData>>()
            .ExpandChanges();
    }

    private async Task LoadApplicationAsync()
    {
        await using var scope = _host.Services.CreateAsyncScope();
        await LoadAppDataAsync(scope);
        CultureInfo.CurrentUICulture =
            await scope.ServiceProvider
                .GetRequiredService<IUserJsonManager>()
                .GetCurrentCultureAsync();
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture;
    }

    private ValueTask LoadAppDataAsync(IServiceScope scope)
    {
        return scope.ServiceProvider
            .GetRequiredService<IUserJsonCreator>()
            .CreateAsync();
    }

    private void RegisterExceptionView()
    {
        DispatcherUnhandledException += (_, args) =>
        {
            if (args.Exception is UnauthorizedAccessException)
            {
                
                args.Handled = true;
                return;
            }
            
            var logger = Ioc.Default.GetRequiredService<ILogger<App>>();

            logger.LogCritical(args.Exception, "Critical exception");


            Ioc.Default
                .GetRequiredService<IDialogHelper>()
                .Show(ApplicationViews.Error);

            args.Handled = true;
        };
    }
}