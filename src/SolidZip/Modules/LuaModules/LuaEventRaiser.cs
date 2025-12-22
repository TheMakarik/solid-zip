namespace SolidZip.Modules.LuaModules;

public sealed class LuaEventRaiser(ILuaEvents events, 
    ILuaDebugConsole console,
    ILogger<LuaEventRaiser> logger,
    ILuaGlobalsLoader globalsLoader) : ILuaEventRaiser
{
    public async ValueTask RaiseAsync<T>(string @event, T args)
    {
        await ExecuteForExtensionsAsync(@event, (lua, extension) =>
        {
            lua["__args"] = args;
            lua.DoString($"script.on_{@event}(__args or nil);");
        });
    }
    
    public async ValueTask RaiseBackground<T>(string @event, T args)
    {
        await ExecuteBackgroundAsync(@event, (lua, extension) =>
        {
            lua["__args"] = args;
            lua.DoString($"script.on_{@event}(__args or nil);");
        });
    }

    public async ValueTask<TReturn[]> RaiseAsync<TReturn, TArgs>(string @event, TArgs args)
    {
        return await ExecuteWithReturnAsync<TReturn, TArgs>(@event += "_ret", args, (lua, arg) =>
        {
            lua["__args"] = arg;
            var result = lua.DoString($"return script.on_{@event}(__args or nil);");
            return result.Select(r => (TReturn)r).ToArray();
        });
    }

    public async ValueTask RaiseAsync(string @event)
    {
        await ExecuteForExtensionsAsync(@event, (lua, extension) =>
        {
            lua.DoString($"script.on_{@event}();");
        });
    }

    public async ValueTask RaiseBackground(string @event)
    {
        await ExecuteBackgroundAsync(@event, (lua, extension) =>
        {
            lua.DoString($"script.on_{@event}();");
        });
    }

    public async ValueTask<TReturn[]> RaiseAsync<TReturn>(string @event)
    {
        return await ExecuteWithReturnAsync<TReturn, object>(@event += "_ret", null!, (lua, arg) =>
        {
            var result = lua.DoString($"return script.on_{@event}();");
            return result.Select(r => (TReturn)r).ToArray();
        });
    }

    private async ValueTask ExecuteForExtensionsAsync(string @event, Action<Lua, string> executeAction)
    {
        var extensions = events.Get(@event);
        if (!extensions.Any())
            return;
        
        logger.LogDebug("Raising lua event: {event} with {extensions} files", @event, extensions);

        var tasks = extensions.Select(extension => Task.Run(() =>
        {
            ExecuteLuaScript(extension, lua => executeAction(lua, extension));
        }));

        await Task.WhenAll(tasks);
    }

    private async ValueTask<TReturn[]> ExecuteWithReturnAsync<TReturn, TArgs>(string @event, TArgs args, Func<Lua, TArgs, TReturn[]> executeFunc)
    {
        var extensions = events.Get(@event);
        if (!extensions.Any())
            return [];
        
        logger.LogDebug("Raising lua event with return: {event} with {extensions} files", @event, extensions);

        var tasks = extensions.Select(extension => Task.Run(() =>
        {
            return ExecuteLuaScriptWithReturn(extension, lua => executeFunc(lua, args));
        }));

        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r).ToArray();
    }

    private async ValueTask ExecuteBackgroundAsync(string @event, Action<Lua, string> executeAction)
    {
        var extensions = events.Get(@event);
        if (!extensions.Any())
            return;
        
        logger.LogDebug("Raising background lua event: {event} with {extensions} files", @event, extensions);

        await ExecuteForExtensionsAsync(@event, executeAction);
    }

    private void ExecuteLuaScript(string extension, Action<Lua> executeAction)
    {
        using var lua = new Lua();
        globalsLoader.Load(lua, extension);
        try
        {
            lua.DoFile(extension);
            executeAction(lua);
        }
        catch (Exception exception)
        {
            var exceptionMessage = $"Exception occured: {exception.Message} {exception.InnerException}";
            console.PrintAsync(exceptionMessage, extension, ConsoleColor.Red);
            logger.LogError("{message} at path {path}", exceptionMessage, extension);
        }
    
    }

    private TReturn[] ExecuteLuaScriptWithReturn<TReturn>(string extension, Func<Lua, TReturn[]> executeFunc)
    {
        using var lua = new Lua();
        globalsLoader.Load(lua, extension);
        try
        {
          
            lua.DoFile(extension);
            return executeFunc(lua);
        }
        catch (Exception exception)
        {
            var exceptionMessage = $"Exception occured: {exception}";
            console.PrintAsync(exceptionMessage, extension, ConsoleColor.Red);
            logger.LogError(exception.InnerException, "{message} at path {path}", exceptionMessage, extension);
            return [];
        }
    }
    
}