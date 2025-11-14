using Serilog;
using SolidZip.Localization;

namespace SolidZip;

public class Startup
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
            .AddThemes((value, key) =>  Application.Current.Resources[key] = value)
            .AddViewModelLocator()
            .AddSingleton<RetrySystem>()
            .AddWin32()
            .AddSingleton<StrongTypedLocalizationManager>()
            .AddLua()
            .AddAppData()
            .AddPathsUtils()
            .AddCache<UserData>();
        
        return hostBuilder.Build();
    }
    
}