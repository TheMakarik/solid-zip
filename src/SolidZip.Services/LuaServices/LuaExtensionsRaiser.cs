namespace SolidZip.Services.LuaServices;

public class LuaExtensionsRaiser(
    ILogger<LuaExtensionsRaiser> logger, 
    LuaFactory luaFactory, 
    IOptions<LuaConfiguration> luaConfiguration,
    ILuaExtensions extensions) : ILuaExtensionsRaiser
{
    private const string RaisingEventLogMessage = "Raising {name} event..";
    
    public void RaiseBackground(string eventName)
    {
        RaiseTask(eventName);
    }

    public async Task RaiseAsync(string eventName)
    {
        await RaiseTask(eventName);
    }

    public async IAsyncEnumerable<T> RaiseAsync<T>(string eventName)
    {
        logger.LogDebug(RaisingEventLogMessage, eventName);

        foreach (var extension in extensions.GetLuaExtensions(eventName))
        {
            using var lua = luaFactory.GetFactory(extension);
            await Task.Run(() => lua.DoFile(extension));
            var script = lua[luaConfiguration.Value.LuaScriptName];
            var action = script[luaConfiguration.Value.ExecuteFunctionName] as Func<T>;
            var result = action();
            yield return result;
        };
    }

    private Task RaiseTask(string eventName)
    {
        return Task.Factory.StartNew(() =>
        {
            logger.LogInformation(RaisingEventLogMessage, eventName);
            Parallel.ForEach(extensions.GetLuaExtensions(eventName), (extension, state) =>
            {
                using var lua = luaFactory.GetFactory(extension);
                lua.DoFile(extension);
                var script = lua[luaConfiguration.Value.LuaScriptName];
                script.InvokeFunction(luaConfiguration.Value.ExecuteFunctionName);
            });

        }, TaskCreationOptions.LongRunning);
    }
    
}