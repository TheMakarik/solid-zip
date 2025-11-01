namespace SolidZip.Services.LuaServices;

//Untestable due to luaFactory
internal sealed class LuaExtensionsRaiser(
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
        bool wasNotLogged = true;
        
        foreach (var extension in extensions.GetLuaExtensions(eventName))
        {
            //Do it only for extensions that's exists
            if (wasNotLogged)
            {
                logger.LogInformation(RaisingEventLogMessage, eventName);
                wasNotLogged = false;
            }
         
            
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
        bool  wasNotLogged = true;
        
        return Task.Factory.StartNew(() =>
        {
            Parallel.ForEach(extensions.GetLuaExtensions(eventName), (extension, state) =>
            {
                //Do it only for extensions that's exists
                if (wasNotLogged)
                {
                    logger.LogInformation(RaisingEventLogMessage, eventName);
                    wasNotLogged = false;
                }
                
                using var lua = luaFactory.GetFactory(extension);
                lua.DoFile(extension);
                var script = lua[luaConfiguration.Value.LuaScriptName];
                script.InvokeFunction(luaConfiguration.Value.ExecuteFunctionName);
            });

        }, TaskCreationOptions.LongRunning);
    }
    
}