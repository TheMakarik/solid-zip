namespace SolidZip.Services.LuaServices;

internal sealed class LuaGlobalsLoader(
    ILogger<LuaGlobalsLoader> logger,
    ILoggerFactory factory,
    IDirectoryProxy directoryProxy,
    IOptions<LuaConfiguration> luaConfiguration,
    IServiceProvider services) : ILuaGlobalsLoader
{
    private const string ErrorGettingServiceLogMessage = "Error getting service {TypeName}";
    private const string LoadingLuaModulesFolderLogMessage = "Loading Lua modules folder {path}";
    private const string UnexistingModulesFolderLogMessage = "Modules folder does not exist: {Folder}";
    
    public void Load<T>(LuaConnection<T> lua, string scriptPath)
    {
        var pathBuilder = LoadModules(lua);
        
        lua.DoString($"package.path = package.path .. '{pathBuilder}'");
        
        LoadLogger(lua, scriptPath);
        lua.SetIndexer("fromSz", new Func<string, object>(GetServiceByTypeName));  
    }

    private StringBuilder LoadModules<T>(LuaConnection<T> lua)
    {
        var pathBuilder = new StringBuilder();
        foreach (var modulesFolder in luaConfiguration.Value.Modules ?? Enumerable.Empty<string>())
        {
            if (string.IsNullOrEmpty(modulesFolder)) continue;
        
            if (directoryProxy.Exists(modulesFolder))
            {
                var normalizedPath = modulesFolder.Replace("\\", "/");
                pathBuilder.Append($";{normalizedPath}/?.lua");                   
                pathBuilder.Append($";{normalizedPath}/**/?.lua");                 
                logger.LogTrace(LoadingLuaModulesFolderLogMessage, modulesFolder);
            }
            else
                logger.LogWarning(UnexistingModulesFolderLogMessage, modulesFolder);
        }
        return pathBuilder;
    }

    private object GetServiceByTypeName(string typeName)
    {
        try
        {
            var type = Type.GetType(typeName) ?? AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);

            if (type is null)
                throw new InvalidOperationException($"Type '{typeName}' not found");

            var service = services.GetService(type);

            if (service is null)
                throw new InvalidOperationException($"Service of type '{typeName}' not registered in DI container");

            return service;
        }
        catch (Exception ex)
        {
            var logger = services.GetService<ILogger<LuaGlobalsLoader>>();
            logger?.LogError(ex, ErrorGettingServiceLogMessage, typeName);
            throw;
        }
    }

    private void LoadLogger<T>(LuaConnection<T> lua, string scriptPath)
    {
        var scriptLogger = factory.CreateLogger(scriptPath);
        
        lua.SetIndexer("_info", (string message) => scriptLogger.LogInformation(message)); 
        lua.SetIndexer("_debug", (string message) => scriptLogger.LogDebug(message)); 
        lua.SetIndexer("_trace", (string message) => scriptLogger.LogTrace(message)); 
        lua.SetIndexer("_warn", (string message) => scriptLogger.LogWarning(message)); 
        lua.SetIndexer("_error", (string message) => scriptLogger.LogError(message)); 
        lua.SetIndexer("_critical", (string message) => scriptLogger.LogCritical(message)); 
    }

}