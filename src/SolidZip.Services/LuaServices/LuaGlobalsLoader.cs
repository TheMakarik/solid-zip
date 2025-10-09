using System.Text;
using SolidZip.Services.LuaServices.Abstraction;
using SolidZip.Services.ProxiesServices.Abstractions;

namespace SolidZip.Services.LuaServices;

public class LuaGlobalsLoader(
    ILogger<LuaGlobalsLoader> logger,
    ILoggerFactory factory,
    IDirectoryProxy directoryProxy,
    IOptions<LuaConfiguration> luaConfiguration,
    IServiceProvider services) : ILuaGlobalsLoader
{
    private const string ErrorGettingServiceLogMessage = "Error getting service {TypeName}";
    private const string LoadingLuaModulesFolderLogMessage = "Loading Lua modules folder {path}";
    private const string UnexistingModulesFolderLogMessage = "Modules folder does not exist: {Folder}";
    
    public void Load(Lua lua, string scriptPath)
    {
        var pathBuilder = LoadModules(lua);
        
        lua.DoString($"package.path = package.path .. '{pathBuilder}'");

        lua.LoadCLRPackage();
        LoadLogger(lua, scriptPath);
        lua["fromSz"] = new Func<string, object>(GetServiceByTypeName);
    }

    private StringBuilder LoadModules(Lua lua)
    {
        var pathBuilder = new StringBuilder();
        foreach (var modulesFolder in luaConfiguration.Value.Modules ?? Enumerable.Empty<string>())
        {
            if (string.IsNullOrEmpty(modulesFolder)) continue;
        
            if (directoryProxy.Exists(modulesFolder))
            {
                var normalizedPath = modulesFolder.Replace("\\", "/");
                pathBuilder.Append($";{normalizedPath}/?.lua");
                logger.LogTrace(LoadingLuaModulesFolderLogMessage, modulesFolder);
            }
            else
            {
                logger.LogWarning(UnexistingModulesFolderLogMessage, modulesFolder);
            }
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

    private void LoadLogger(Lua lua, string scriptPath)
    {
        var logger = factory.CreateLogger(scriptPath);
        
        lua["_info"] = (string message) => { logger.LogInformation(message); };
        lua["_debug"] = (string message) => { logger.LogDebug(message); };
        lua["_trace"] = (string message) => { logger.LogTrace(message); };
        lua["_warn"] = (string message) => { logger.LogWarning(message); };
        lua["_error"] = (string message) => { logger.LogError(message); };
        lua["_critical"] = (string message) => { logger.LogCritical(message); };
    }

}