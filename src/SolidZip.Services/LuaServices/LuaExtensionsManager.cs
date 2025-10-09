using System.Collections.Concurrent;
using System.Collections.Frozen;
using SolidZip.Services.LuaServices.Abstraction;
using SolidZip.Services.ProxiesServices.Abstractions;

namespace SolidZip.Services.LuaServices;

public class LuaExtensionsManager(
    ILogger<LuaExtensionsManager> logger, 
    IDirectoryProxy directoryProxy, 
    IOptions<LuaConfiguration> luaConfiguration,
    INLuaEngine nLuaEngine) : ILuaExtensionsManager
{
    private const string StartSearchingExtensionsLogMessage = "Start searching lua extensions";
    private const string SearchedAllExtensionsLogMessage = "Searched {total} extensions, for {time} milleseconds";
    private const string LoadedScriptFolderLogMessage = "Loaded script folder {path}";
    private const string TotalFilesToProcessLogMessage = "Total files to process: {count}";
    private const string StartingSequentialProcessingLogMessage = "Starting sequential processing of {count} files";
    private const string ProcessingFileLogMessage = "Processing file: {file}";
    private const string AttemptingToExecuteLuaScriptLogMessage = "Attempting to execute Lua script: {file}";
    private const string ScriptReturnedNullLogMessage = "Script returned null for: {path}";
    private const string ScriptLoadedSuccessfullyLogMessage = "Script loaded successfully: {file}";
    private const string FoundEventsLogMessage = "Found events: {events} in {file}";
    private const string RegisteredFileForEventLogMessage = "Registered {file} for event {event}";
    private const string FinalExtensionsCountLogMessage = "Final extensions count: {count}";
    private const string EventScriptsLogMessage = "Event: {event}, Scripts: {scripts}";
    private const string FrozenDictionaryCreatedLogMessage = "FrozenDictionary created with {count} events";
    private const string FailedToProcessLuaScriptLogMessage = "Failed to process Lua script: {path}";

    private FrozenDictionary<string, List<string>> _extensionsOnEvent;

    public IEnumerable<string> GetEventScripts(string eventName)
    {
        return _extensionsOnEvent[eventName];
    }

    public async Task SubscribeAllAsync()
    {
        logger.LogDebug(StartSearchingExtensionsLogMessage);
        var stopwatch = Stopwatch.StartNew();

        await SubscribeAllWithoutTimer();
        
        logger.LogInformation(SearchedAllExtensionsLogMessage, _extensionsOnEvent.Count, stopwatch.ElapsedMilliseconds);
    }

    private async Task SubscribeAllWithoutTimer()
    {
        var extensionsOnEvent = new Dictionary<string, List<string>>();

        var allExtensions = luaConfiguration.Value.ScriptsFolders
            .SelectMany(folder =>
            {
                logger.LogDebug(LoadedScriptFolderLogMessage, folder);
                return directoryProxy.EnumerateFiles(
                    folder,
                    luaConfiguration.Value.LuaExtensionsPattern,
                    SearchOption.AllDirectories);
            })
            .ToList();

        logger.LogTrace(TotalFilesToProcessLogMessage, allExtensions.Count);
        logger.LogTrace(StartingSequentialProcessingLogMessage, allExtensions.Count);

        foreach (var extensionPath in allExtensions)
        {
            logger.LogTrace(ProcessingFileLogMessage, extensionPath);
            
            try
            {
                logger.LogTrace(AttemptingToExecuteLuaScriptLogMessage, extensionPath);
                
                var script = nLuaEngine.Execute(extensionPath).FirstOrDefault() as LuaTable;
                if (script == null)
                {
                    logger.LogWarning(ScriptReturnedNullLogMessage, extensionPath);
                    continue;
                }

                logger.LogTrace(ScriptLoadedSuccessfullyLogMessage, extensionPath);

                var events = script[luaConfiguration.Value.EventsField] as IEnumerable<string>;
                
                logger.LogTrace(FoundEventsLogMessage, string.Join(", ", events), extensionPath);

                foreach (var eventName in events)
                {
                    if (!extensionsOnEvent.ContainsKey(eventName))
                        extensionsOnEvent[eventName] = new List<string>();
                    
                    extensionsOnEvent[eventName].Add(extensionPath);
                    logger.LogTrace(RegisteredFileForEventLogMessage, extensionPath, eventName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, FailedToProcessLuaScriptLogMessage, extensionPath);
            }
        }

        logger.LogTrace(FinalExtensionsCountLogMessage, extensionsOnEvent.Count);
        foreach (var extensionAndEventPair in extensionsOnEvent)
            logger.LogTrace(EventScriptsLogMessage, extensionAndEventPair.Key, string.Join(", ", extensionAndEventPair.Value));

        _extensionsOnEvent = extensionsOnEvent.ToFrozenDictionary();
        logger.LogTrace(FrozenDictionaryCreatedLogMessage, _extensionsOnEvent.Count);
    }
}