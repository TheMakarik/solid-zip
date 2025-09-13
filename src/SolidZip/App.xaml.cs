using System.IO;
using SolidZip.Localization;
using SolidZip.Model.Options;
using SolidZip.Services;

namespace SolidZip;

public partial class App 
{
    private const string ConfigurationPath = "configuration";
    private const string JsonExtension = ".json";
    
    private readonly IHost _app;
  
    public App()
    {
        var builder = Host.CreateApplicationBuilder();

        Directory.EnumerateFiles(ConfigurationPath)
            .Where(file => Path.GetExtension(file) == JsonExtension)
            .ForEach(path => builder.Configuration.AddJsonFile(path))
            .ToList();
        
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
            .AddSingleton<StrongTypedLocalizationManager>()
            .Bind<MainView, MainViewModel>();
        
        builder.Logging
            .ClearProviders()
            .AddSerilog(Log.Logger, dispose: true);

        _app = builder.Build();
        _app.RunAsync();
       
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _app.Services.GetRequiredService<IAppDataContentCreator>().CreateAsync();
        _app.Services.GetWindow<MainView>().Show();
        base.OnStartup(e);
    }
}