using System.Collections.Concurrent;
using System.Collections.Frozen;
using SolidZip.Services.LuaServices.Abstraction;
using SolidZip.Services.ProxiesServices.Abstractions;

namespace SolidZip.Services.LuaServices;

public class LuaExtensionsSubscriber(
    ILogger<LuaExtensionsSubscriber> logger, 
    IDirectoryProxy directoryProxy, 
    IOptions<LuaConfiguration> luaConfiguration,
    INLuaEngine nLuaEngine) : ILuaExtensionsSubscriber
{
    private const string StartSearchingExtensionsLogMessage = "Start searching lua extensions";
    private const string SearchedAllExtensionsLogMessage = "Searched {total} extensions, for {time} milleseconds";
    private const string StartingEventLogMessage = "Is starting {eventName} event";
    private const string LoadedScriptFolderLogMessage = "Loaded script folder {path}";
    
    internal FrozenDictionary<string, ConcurrentBag<string>> ExtensionsOnEvent { get; private set; } 
    
    public async Task SubscribeAllAsync()
    {
        logger.LogDebug(StartSearchingExtensionsLogMessage);
        var stopwatch = Stopwatch.StartNew();

        await SubscribeAllWithoutTimer();
        
        logger.LogInformation(SearchedAllExtensionsLogMessage, ExtensionsOnEvent.Count, stopwatch.ElapsedMilliseconds);
        
    }

    private async Task SubscribeAllWithoutTimer()
    {
        var extensionsOnEvent = new ConcurrentDictionary<string, ConcurrentBag<string>>();

        await Parallel.ForEachAsync(luaConfiguration.Value.ScriptsFolders, async (folder, token) =>
        {
            logger.LogDebug(LoadedScriptFolderLogMessage, folder);
            
            var extensions = directoryProxy.EnumerateFiles(folder, 
                luaConfiguration.Value.LuaExtensionsPattern,
                SearchOption.AllDirectories);
            
            await Parallel.ForEachAsync(extensions, token, async (extensionPath, cancellationToken) =>
            {
                var script = await Task
                    .Run(() => (LuaTable)nLuaEngine.Execute(extensionPath).FirstOrDefault()!, cancellationToken);
                
                var events = script[luaConfiguration.Value.EventsField] as IEnumerable<string> ?? [];
                foreach(var eventName in events)
                    if (!extensionsOnEvent.TryAdd(eventName, new ConcurrentBag<string>([extensionPath])))
                    {
                        var eventsExtensions = extensionsOnEvent.GetValueOrDefault(eventName)!;
                        eventsExtensions.Add(extensionPath);
                        extensionsOnEvent.TryUpdate(eventName, eventsExtensions, extensionsOnEvent.GetValueOrDefault(eventName)!);
                    }
            });
        });
        
        ExtensionsOnEvent = extensionsOnEvent.ToFrozenDictionary();
    }
}