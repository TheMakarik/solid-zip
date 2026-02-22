namespace SolidZip.Modules.LuaModules;

public sealed class LuaEventRaiser(
    ILuaEvents events,
    ILuaDebugConsole console,
    ILogger<ILuaEventRaiser> logger,
    ILuaGlobalsLoader globalsLoader,
    ILuaStateCaching stateCaching) : ILuaEventRaiser
{
    public async ValueTask RaiseAsync<T>(string @event, T args)
    {
        await ExecuteForExtensionsAsync(@event, (lua, extension, fromCache) =>
        {
            lua["__args"] = args;
            lua["__event"] = @event;
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.start ~= nil then
                        script.start('{@event}');
                    end
                ");
            }
            
            lua.DoString($@"
                if script.start_exactly ~= nil then
                    script.start_exactly('{@event}');
                end
                script.on_{@event}(__args or nil);
                if script.stop_exactly ~= nil then
                    script.stop_exactly('{@event}');
                end
            ");
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.stop ~= nil then
                        script.stop('{@event}');
                    end
                ");
            }
        });
    }

    public async ValueTask RaiseBackground<T>(string @event, T args)
    {
        await ExecuteBackgroundAsync(@event, (lua, extension, fromCache) =>
        {
            lua["__args"] = args;
            lua["__event"] = @event;
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.start ~= nil then
                        script.start('{@event}');
                    end
                ");
            }
            
            lua.DoString($@"
                if script.start_exactly ~= nil then
                    script.start_exactly('{@event}');
                end
                script.on_{@event}(__args or nil);
                if script.stop_exactly ~= nil then
                    script.stop_exactly('{@event}');
                end
            ");
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.stop ~= nil then
                        script.stop('{@event}');
                    end
                ");
            }
        });
    }

    public async ValueTask<TReturn[]> RaiseAsync<TReturn, TArgs>(string @event, TArgs args)
    {
        return await ExecuteWithReturnAsync<TReturn, TArgs>(@event += "_ret", args, (lua, arg, fromCache) =>
        {
            lua["__args"] = arg;
            lua["__event"] = @event;
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.start ~= nil then
                        script.start('{@event}');
                    end
                ");
            }
            
            var result = lua.DoString($@"
                if script.start_exactly ~= nil then
                    script.start_exactly('{@event}');
                end
                local ret = script.on_{@event}(__args or nil);
                if script.stop_exactly ~= nil then
                    script.stop_exactly('{@event}');
                end
                return ret;
            ");
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.stop ~= nil then
                        script.stop('{@event}');
                    end
                ");
            }
            
            return result.Select(r => ProcessLuaResult<TReturn>(r)).ToArray();
        });
    }

    public async ValueTask RaiseAsync(string @event)
    {
        await ExecuteForExtensionsAsync(@event, (lua, extension, fromCache) =>
        {
            lua["__event"] = @event;
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.start ~= nil then
                        script.start('{@event}');
                    end
                ");
            }
            
            lua.DoString($@"
                if script.start_exactly ~= nil then
                    script.start_exactly('{@event}');
                end
                script.on_{@event}();
                if script.stop_exactly ~= nil then
                    script.stop_exactly('{@event}');
                end
            ");
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.stop ~= nil then
                        script.stop('{@event}');
                    end
                ");
            }
        });
    }

    public async ValueTask RaiseBackground(string @event)
    {
        await ExecuteBackgroundAsync(@event, (lua, extension, fromCache) =>
        {
            lua["__event"] = @event;
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.start ~= nil then
                        script.start('{@event}');
                    end
                ");
            }
            
            lua.DoString($@"
                if script.start_exactly ~= nil then
                    script.start_exactly('{@event}');
                end
                script.on_{@event}();
                if script.stop_exactly ~= nil then
                    script.stop_exactly('{@event}');
                end
            ");
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.stop ~= nil then
                        script.stop('{@event}');
                    end
                ");
            }
        });
    }

    public async ValueTask<TReturn[]> RaiseAsync<TReturn>(string @event)
    {
        return await ExecuteWithReturnAsync<TReturn, object>(@event += "_ret", null!, (lua, arg, fromCache) =>
        {
            lua["__event"] = @event;
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.start ~= nil then
                        script.start('{@event}');
                    end
                ");
            }
            
            var result = lua.DoString($@"
                if script.start_exactly ~= nil then
                    script.start_exactly('{@event}');
                end
                local ret = script.on_{@event}();
                if script.stop_exactly ~= nil then
                    script.stop_exactly('{@event}');
                end
                return ret;
            ");
            
            if (!fromCache)
            {
                lua.DoString($@"
                    if script.stop ~= nil then
                        script.stop('{@event}');
                    end
                ");
            }
            
            return result.Select(r => ProcessLuaResult<TReturn>(r)).ToArray()!;
        });
    }

    private TReturn? ProcessLuaResult<TReturn>(object result)
    {
        if (result is not LuaTable table) return (TReturn)result;
        if (table["register"] is not LuaFunction registerFunction)
            return (TReturn?)(object)ToDictionary(table);
        var registerResult = registerFunction.Call(table);
        if (registerResult is { Length: > 0 })
        {
            return (TReturn)registerResult[0];
        }
        return default(TReturn);


    }

    private IDictionary ToDictionary(LuaTable table)
    {
        var dictionary = new Dictionary<object, object>();
        foreach (var key in table.Keys)
        {
            if (table[key] is LuaFunction)
                throw new InvalidOperationException("Cannot return lua table with functions(not ui component)");
            
            dictionary[key] = key is LuaTable innerTable
                ? ToDictionary(innerTable) 
                : table[key];
        }
        return dictionary;
    }

    private async ValueTask ExecuteForExtensionsAsync(string @event, Action<Lua, string, bool> executeAction)
    {
        var extensions = events.Get(@event);
        if (!extensions.Any())
            return;

        logger.LogDebug("Raising lua event: {event} with {extensions} files", @event, extensions);

        var tasks = extensions.Select(extension => Task.Run(() =>
        {
            ExecuteLuaScript(extension, lua => executeAction(lua, extension, false), (lua, fromCache) => executeAction(lua, extension, fromCache));
        }));

        await Task.WhenAll(tasks);
    }

    private async ValueTask<TReturn[]> ExecuteWithReturnAsync<TReturn, TArgs>(string @event, TArgs args,
        Func<Lua, TArgs, bool, TReturn[]> executeFunc)
    {
        var extensions = events.Get(@event);
        if (!extensions.Any())
            return [];

        logger.LogDebug("Raising lua event with return: {event} with {extensions} files", @event, extensions);

        var tasks = extensions.Select(extension => Task.Run(() =>
        {
            return ExecuteLuaScriptWithReturn(extension, lua => executeFunc(lua, args, false), (lua, fromCache) => executeFunc(lua, args, fromCache));
        }));

        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r).ToArray();
    }

    private async ValueTask ExecuteBackgroundAsync(string @event, Action<Lua, string, bool> executeAction)
    {
        var extensions = events.Get(@event);
        if (!extensions.Any())
            return;

        logger.LogDebug("Raising background lua event: {event} with {extensions} files", @event, extensions);

        await ExecuteForExtensionsAsync(@event, executeAction);
    }

    private void ExecuteLuaScript(string extension, Action<Lua> createAction, Action<Lua, bool> executeAction)
    {
        var cachedLua = stateCaching.Get(extension);
        
        if (cachedLua != null)
        {
            logger.LogDebug("Using cached Lua state for {extension}", extension);
            executeAction(cachedLua, true);
        }
        else
        {
         
            logger.LogDebug("Creating new Lua state for {extension}", extension);
            using var lua = new Lua();
            globalsLoader.Load(lua, extension);
            
            try
            {
                lua.DoFile(extension);
                createAction(lua);
                
                executeAction(lua, false);
            }
            catch (Exception exception)
            {
                var exceptionMessage =
                    $"Exception occured: {exception.Message} {exception.InnerException}\nSTACK_TRACE: {exception.StackTrace}";
                console.PrintAsync(exceptionMessage, extension, ConsoleColor.Red);
                logger.LogError("{message} at path {path}", exceptionMessage, extension);
            }
        }
    }

    private TReturn[] ExecuteLuaScriptWithReturn<TReturn>(string extension, Func<Lua, TReturn[]> createFunc, Func<Lua, bool, TReturn[]> executeFunc)
    {
        var cachedLua = stateCaching.Get(extension);
        
        if (cachedLua != null)
        {
            logger.LogDebug("Using cached Lua state for {extension}", extension);
            return executeFunc(cachedLua, true);
        }

        logger.LogDebug("Creating new Lua state for {extension}", extension);
        using var lua = new Lua();
        globalsLoader.Load(lua, extension);
            
        try
        {
            lua.DoFile(extension);
            var result = createFunc(lua);
                
            return result;
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