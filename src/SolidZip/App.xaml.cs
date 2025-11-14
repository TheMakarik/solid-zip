namespace SolidZip;

public partial class App
{
    private readonly IHost _host = new Startup().BuildHost();
    private ILogger<App> _logger;

    protected override async void OnStartup(StartupEventArgs e)
    {
        Ioc.Default.ConfigureServices(_host.Services);
        _logger = _host.Services.GetRequiredService<ILogger<App>>();
        
        if(e.Args.Any())
            _logger.LogDebug("Startup args: {args}", e.Args);
        
        var task = LoadLuaPlugins();
        await LoadApplicationAsync();
        AttachLuaConsole();
        await task;
        await LoadThemeAsync();
        await RaiseLuaEventsAsync();
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
       await raiser.Raise("init")
           .AsTask()
           .ContinueWith(async (task) =>
           {
               await raiser.RaiseBackground("startup");
           });
       
    }

    private Task LoadLuaPlugins()
    {
        return _host.Services.GetRequiredService<ILuaEventLoader>()
            .LoadAsync(new Progress<double>(), 50); //To do: implement progress bar
    }

    private void AttachLuaConsole()
    {
        var console = _host.Services.GetRequiredService<ILuaDebugConsole>();
        console.AttachAsync();
    }

    protected sealed override async void OnExit(ExitEventArgs e)
    {
        var raiser = _host.Services.GetRequiredService<ILuaEventRaiser>();
        var task =  raiser.Raise("exit", e.ApplicationExitCode);
        ExpandUserData();
        await task;
        base.OnExit(e);
    }

    private void ExpandUserData()
    {
        _host.Services.GetRequiredService<SharedCache<UserData>>().ExpandChanges(data =>
        {
            using var stream = new FileStream(
                _host.Services.GetRequiredService<PathsCollection>().UserData,
                FileMode.Truncate);
            
            JsonSerializer.Serialize(stream, data);
        });
    }

    private async Task LoadApplicationAsync()
    {
        await using var scope = _host.Services.CreateAsyncScope();
        await LoadAppDataAsync(scope);
    }

    private ValueTask LoadAppDataAsync(IServiceScope scope)
    {
        return scope.ServiceProvider
            .GetRequiredService<IUserJsonCreator>()
            .CreateAsync();
    }
}