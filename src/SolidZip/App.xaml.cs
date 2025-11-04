namespace SolidZip;

public partial class App
{
    private readonly IHost _host = new Startup().BuildHost();
    private ILogger<App> _logger;

    protected override async void OnStartup(StartupEventArgs e)
    {
        _logger = _host.Services.GetRequiredService<ILogger<App>>();
        _logger.LogDebug("Application is starting"); //DO NOT DELETE THIS LOG, Serilog will create directory for logging and solid-zip appdata unless it created
        
        if(e.Args.Any())
            _logger.LogDebug("Startup args: {args}", e.Args);
        
        await LoadApplicationAsync();
        
        base.OnStartup(e);
    }

    protected sealed override void OnExit(ExitEventArgs e)
    {
        _logger.LogDebug("Application was stopped with exit code {code}", e.ApplicationExitCode);
        ExpandUserData();
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