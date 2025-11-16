
namespace SolidZip.Modules.LuaModules;

public class LuaGlobalsLoader(
     ILoggerFactory loggerFactory, 
     ILogger<LuaGlobalsLoader> logger,
     IServiceProvider provider,
     ILuaDebugConsole console,
     ILuaShared luaShared,
     PathsCollection paths) : ILuaGlobalsLoader
{
     public void Load(Lua lua, string scriptPath)
    {
         var stopwatch = Stopwatch.StartNew();
         try
         {
              LoadPackages(lua);
              lua.LoadCLRPackage();

              LoadLogger(lua, scriptPath);
              LoadExternFunctions(lua);
              LoadDebugging(lua, scriptPath);
              LoadShared(lua);
              LoadScriptTable(lua);
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
     
     private void LoadShared(Lua lua)
     {
          lua["_get_shared"] = (string key) => 
          {
               var result = luaShared.Get(key);
               logger.LogDebug("Lua requested shared key: {key} -> {result}", key, result ?? "null");
               return result;
          };
    
          lua["_set_shared"] = (string key, object value) => 
          {
               logger.LogDebug("Lua setting shared key: {key} = {value} (Type: {type})", 
                    key, value, value?.GetType().Name ?? "null");
               luaShared.Add(key, value);
          };
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
           
           script.logger = {}
           script.debug = {}
           script.extern = {}
           script.shared = {}

           local shared_mt = {}
           shared_mt.__index = function(_, key) 
                  return _G._get_shared(key);
           end
           shared_mt.__newindex = function(_, key, val) 
                  _G._set_shared(key, val)
           end
           setmetatable(script.shared, shared_mt)
           
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