namespace SolidZip.Modules.LuaModules;

public sealed class LuaGlobalsLoader(
    ILoggerFactory loggerFactory,
    ILogger<LuaGlobalsLoader> logger,
    IServiceProvider provider,
    ILuaDebugConsole console,
    ILuaShared luaShared,
    ILuaUiData uiData,
    LuaEventRedirector eventRedirector,
    MaterialIconLuaLoader materialIconLuaLoader,
    ILuaStateCaching luaStateCaching,
    PathsCollection paths) : ILuaGlobalsLoader
{
    public void Load(Lua lua, string scriptPath)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            lua.State.Encoding = Encoding.UTF8;
            LoadPackages(lua);
            lua.LoadCLRPackage();

            LoadLogger(lua, scriptPath);
            LoadExternFunctions(lua);
            LoadDebugging(lua, scriptPath);
            LoadSharedAndUi(lua);
            LoadScriptInfo(lua, scriptPath);
            LoadMaterialIconLoader(lua, scriptPath);
            LoadEventRedirector(lua);
            LoadScriptTable(lua);
            
            AddCachingFunctions(lua, scriptPath);
        }
        catch (Exception exception)
        {
            var exceptionMessage = $"Exception occured: {exception.Message}";
            console.PrintAsync(exceptionMessage, scriptPath, ConsoleColor.Red);
            logger.LogError("{message} at path {path}", exceptionMessage, scriptPath);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogDebug("Loading global for {path} time: {ms} ms", scriptPath, stopwatch.ElapsedMilliseconds);
        }
    }

    private void AddCachingFunctions(Lua lua, string scriptPath)
    {
       lua["cache_self"] = () =>
       {
           if(!(bool)((LuaTable)lua.GetTable("script")["RULES"])["NO_CACHE"])
               luaStateCaching.Cache(lua, scriptPath);
       };
       lua["uncache_self"] = () =>  {luaStateCaching.Uncache(lua, scriptPath);};
    }

    private void LoadEventRedirector(Lua lua)
    {
        lua["redirect_to"] = (object eventOwner, string eventName, string luaEventName) =>
            eventRedirector.RedirectEvent(eventOwner, eventName, luaEventName,
                provider.GetRequiredService<ILuaEventRaiser>());
    }

    private void LoadMaterialIconLoader(Lua lua, string scriptPath)
    {
        lua["load_icon"] = (string kind) => materialIconLuaLoader.Load(kind, scriptPath);
    }

    private void LoadScriptInfo(Lua lua, string path)
    {
        if (path.StartsWith("."))
            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + path[1..]; //Cuts the dot 
        else if (path.Contains("%"))
            path = Environment.ExpandEnvironmentVariables(path);

        lua["_path"] = path;
        lua["_folder"] = Path.GetDirectoryName(path);
    }

    private void LoadSharedAndUi(Lua lua)
    {
        lua["_get_shared"] = (string key) =>
        {
            var result = luaShared.Get(key);
            return result;
        };

        lua["_set_shared"] = (string key, object value) => { luaShared.AddOrUpdate(key, value); };

        lua["_get_ui"] = (string key) => uiData.Get(key);
    }

    private void LoadDebugging(Lua lua, string scriptPath)
    {
        lua["_debug_print"] = (string message) => console.PrintAsync(message, scriptPath);
    }

    private void LoadPackages(Lua lua)
    {
        var modulesPath = paths.Modules.ReplaceSeparatorsToAlt();
        lua.DoString($@"
        package.path = package.path .. 
            '{modulesPath}/?.lua;' ..
            '{modulesPath}/?/init.lua;' ..
            '{modulesPath}/?/?.lua'
    ");
    }

    private void LoadExternFunctions(Lua lua)
    {
        lua["_sz"] = (string name) => provider.GetRequiredService(name);
    }

    private void LoadScriptTable(Lua lua)
    {
        lua.DoString(@"
           _G.script = {};

           script.RULES = {};
           local rules_mt = {};
           function rules_mt.__newindex(_, key, val)
                assert(type(val) == 'boolean');
                rawset(_, key, val);
           end       
           setmetatable(script.RULES, rules_mt);     

           script.RULES.FORCE_SHARED_CONTROLS = false;
           script.RULES.NO_CACHE = false;

           _G._SCRIPT = {};

           script.logger = {}
           script.debug = {}
           script.extern = {}
           --shared is outdated
           script.shared = {}
           script.ui = {}


           local _G_mt = {}
           function _G_mt.__newindex(_, key, val)
                if type(val) == 'table' and val.register ~= nil then
                    if not script.RULES.NO_PACK then
                          cache_self()
                    else
                        local converter = require('szlua.private.converter');                 
                        val = converter.table_to_dotnet_dict(val);
                    end
                end
                script.shared[key] = val;
                rawset(_G, key, val)
           end
           function _G_mt.__index(_, key)
                 local shared_key = script.shared[key];
                 if  shared_key ~= nil then
                    return shared_key;
                 end
                 return rawget(_G, key);
           end
           setmetatable(_G, _G_mt);


           local shared_mt = {}

           --shared is outdated
           shared_mt.__index = function(_, key) 
                  return _G._get_shared(key);
           end

           --shared is outdated
           shared_mt.__newindex = function(_, key, val) 
                  _G._set_shared(key, val)
           end
           setmetatable(script.shared, shared_mt)

           local ui_mt = {}
           ui_mt.__index = function(_, key) 
    local res = _G._get_ui(key)
    
    if res ~= nil and res.GetType ~= nil then
        local type_name = res:GetType().Name
        -- Dotnet arrays looks like TypeName[] 
        if #type_name > 2 and type_name:sub(-2) == '[]' then
            local converter = require('szlua.private.converter')
          
            return converter.to_table_from_array(res)
        elseif type_name == 'Dictionary`2' then
            local converter = require('szlua.private.converter')
            return converter.dotnet_dict_to_table(res)
        end
    end
    
    return res
end
           ui_mt.__newindex = function(_, key, val) 
                 error('Cannot create new index at table \'ui\'')
           end
           setmetatable(script.ui, ui_mt)
           
           function script.logger.info(message)
                assert(type(message) == 'string', 'message must be string')
                _G._info(message)
           end

           function script.logger.debug(message)
                assert(type(message) == 'string', 'message must be string')
                _G._debug(message)
           end

           function script.logger.trace(message)
                assert(type(message) == 'string', 'message must be string')
                _G._trace(message)
           end

           function script.logger.warn(message)
                assert(type(message) == 'string', 'message must be string')
                _G._warn(message)
           end

           function script.logger.err(message)
                assert(type(message) == 'string', 'message must be string')
                _G._error(message)
           end

           function script.logger.critical(message)
                assert(type(message) == 'string', 'message must be string')
                _G._critical(message)
           end

           function script.debug.print(message)
                 assert(type(message) == 'string', 'message must be string')
                 _G._debug_print(message)
           end

           function script.extern.sz(name)
                return _G._sz(name)
           end

           function script.extern.using(namespace)
                _G.import(namespace)
           end

           function script.exit(code) 
                code = code or 0
                os.exit(code);
           end

          script.path = _path;
          script.folder = _folder;
       ");
    }

    private void LoadLogger(Lua lua, string scriptPath)
    {
        var logger = loggerFactory.CreateLogger(scriptPath);
        lua["_info"] = (string message) => { logger.LogInformation(message); };
        lua["_debug"] = (string message) => { logger.LogDebug(message); };
        lua["_trace"] = (string message) => { logger.LogTrace(message); };
        lua["_warn"] = (string message) => { logger.LogWarning(message); };
        lua["_error"] = (string message) => { logger.LogError(message); };
        lua["_critical"] = (string message) => { logger.LogCritical(message); };
    }
}