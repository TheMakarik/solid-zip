namespace SolidZip.Modules.LuaModules;

public class LuaGlobalsLoader(ILoggerFactory loggerFactory, 
     ILogger<LuaGlobalsLoader> logger,
     IServiceProvider provider,
     PathsCollection paths) : ILuaGlobalsLoader
{
    private const string InfoLogFunction = "_info";
    private const string DebugLogFunction = "_debug";
    private const string TraceLogFunction = "_trace";
    private const string WarnLogFunction = "_warn";
    private const string ErrorLogFunction = "_error";
    private const string CriticalLogFunction = "_critical";
    private const string ScriptTable = "script";
    private const string DebugPrintFunction = "_debug_print";
    private const string SolidZipImportFunction = "_sz";
    
    
    public void Load(Lua lua, string scriptPath)
    {
         var stopwatch = Stopwatch.StartNew();
         LoadPackages(lua);
         
         lua.LoadCLRPackage();
         
         LoadLogger(lua, scriptPath);
         LoadExternFunctions( lua);
         LoadScriptTable(lua);
         
         stopwatch.Stop();
         logger.LogDebug("Loading global for {path} time: {ms} ms", scriptPath, stopwatch.ElapsedMilliseconds);
    }

    private void LoadPackages(Lua lua)
    {
         lua.DoString($"package.path = package.path .. \"{paths.Modules}/?/.lua;{paths.Modules}/?/?/.lua;{paths.Modules}/?/?/?/.lua\"");
         
    }

    private void LoadExternFunctions(Lua lua)
    {
         lua[SolidZipImportFunction] = (string name) => provider.GetRequiredService(name);
    }

    private void LoadScriptTable(Lua lua)
    {
       lua.DoString(@$"
           _G.{ScriptTable} = {{}};
           
           {ScriptTable}.logger = {{}}
           {ScriptTable}.debug = {{}}
           {ScriptTable}.extern = {{}}
           
           function {ScriptTable}.logger.info(message)
                _G.{InfoLogFunction}(message)
           end

           function {ScriptTable}.logger.debug(message)
                _G.{DebugLogFunction}(message)
           end

           function {ScriptTable}.logger.trace(message)
                _G.{TraceLogFunction}(message)
           end

           function {ScriptTable}.logger.warn(message)
                _G.{WarnLogFunction}(message)
           end

           function {ScriptTable}.logger.err(message)
                _G.{ErrorLogFunction}(message)
           end

           function {ScriptTable}.logger.critical(message)
                _G.{CriticalLogFunction}(message)
           end

           function {ScriptTable}.debug.print(message)
                _G.{DebugPrintFunction}
           end

           function  {ScriptTable}.extern.sz(name)
                return _G.{SolidZipImportFunction}(name)
           end

           function  {ScriptTable}.extern.using_dotnet(namespace)
                return _G.import(namespace)
           end

           function {ScriptTable}.exit(code) 
                code = code or 0
                os.exit(code);
           end
       ");
    }

    private void LoadLogger(Lua lua, string scriptPath)
    {
        var logger = loggerFactory.CreateLogger(scriptPath);
        lua[InfoLogFunction] = (string message) => { logger.LogInformation(message); };
        lua[DebugLogFunction] = (string message) => { logger.LogDebug(message); };
        lua[TraceLogFunction] = (string message) => { logger.LogTrace(message); };
        lua[WarnLogFunction] = (string message) => { logger.LogWarning(message); };
        lua[ErrorLogFunction] = (string message) => { logger.LogError(message); };
        lua[CriticalLogFunction] = (string message) => { logger.LogCritical(message); };
    }
}