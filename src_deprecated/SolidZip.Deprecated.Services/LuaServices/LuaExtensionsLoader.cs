namespace SolidZip.Services.LuaServices;

//Untestable due to luaFactory
internal sealed class LuaExtensionsLoader(
    ILogger<LuaExtensionsLoader> logger, 
    IDirectoryProxy directoryProxy, 
    LuaFactory luaFactory,
    ILuaExtensions luaExtensions,
    IOptions<LuaConfiguration> luaConfiguration) : ILuaExtensionsLoader
{
    private const string StartSearchingExtensionsLogMessage = "Start searching lua extensions";
    private const string SearchedAllExtensionsLogMessage = "Searched {total} extensions, for {time} milleseconds";
    private const string LoadedScriptFolderLogMessage = "Loaded script folder {path}";
    private const string TotalFilesToProcessLogMessage = "Total files to process: {count}";
    private const string ProcessingFileLogMessage = "Processing file: {file}";
    private const string FoundEventsLogMessage = "Found events: {events} in {file}";
    private const string RegisteredFileForEventLogMessage = "Registered {file} for event {event}";
    private const string FinalExtensionsCountLogMessage = "Final extensions count: {count}";
   
    public async Task LoadExtensionsAsync()
    {
        logger.LogInformation(StartSearchingExtensionsLogMessage);
        var stopwatch = Stopwatch.StartNew();

        var total = await LoadExtensionsWithoutTimerAsync();
        stopwatch.Stop();
        logger.LogInformation(SearchedAllExtensionsLogMessage, total, stopwatch.ElapsedMilliseconds);
    }

    private async Task<int> LoadExtensionsWithoutTimerAsync()
    {
        var allExtensions = GetExtensionFiles();
        var extensionsOnEvent = new ConcurrentDictionary<string, ImmutableArray<string>>();
        
        logger.LogTrace(TotalFilesToProcessLogMessage, allExtensions.Count);

        await Parallel.ForEachAsync(allExtensions, (extensionPath, token) =>
        {
            logger.LogTrace(ProcessingFileLogMessage, extensionPath);

            RegisterEvent(extensionsOnEvent, extensionPath);

            return ValueTask.CompletedTask;
        });

        logger.LogTrace(FinalExtensionsCountLogMessage, extensionsOnEvent.Count);
        
        luaExtensions.Load(extensionsOnEvent);
        return extensionsOnEvent.Count;
    }

    private void RegisterEvent(ConcurrentDictionary<string, ImmutableArray<string>> extensionsOnEvent, string extensionPath)
    {
        
        using var lua = luaFactory.GetFactory(extensionPath);
        var events = GetEvents(lua, extensionPath);

        foreach (var eventName in events)
        {
            RegisterEventIfNotExists(extensionsOnEvent, eventName, extensionPath);
                
            extensionsOnEvent[eventName] = extensionsOnEvent[eventName].Add(extensionPath);
            logger.LogTrace(RegisteredFileForEventLogMessage, extensionPath, eventName);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RegisterEventIfNotExists(ConcurrentDictionary<string, ImmutableArray<string>> extensionsOnEvent, string eventName, string extensionPath)
    {
        if (extensionsOnEvent.ContainsKey(eventName))
            return;
        
        var extensions = new List<string>(capacity: 1) { extensionPath };
        extensionsOnEvent[eventName] = extensions.ToImmutableArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IEnumerable<string> GetEvents(LuaConnection<LuaTable> lua, string extensionPath)
    {
        lua.DoFile(extensionPath);

        var script = lua[luaConfiguration.Value.LuaScriptName];
        var eventsEnumerable = script.GetDeeperLuaTableValues(luaConfiguration.Value.EventsField);
        var events = new List<string>(eventsEnumerable.Count);
        foreach (var @event in eventsEnumerable)
        {
            if (@event is string eventName) 
                events.Add(eventName);
        }

        logger.LogTrace(FoundEventsLogMessage, string.Join(", ", events), extensionPath);

        return events;
    }


    private List<string> GetExtensionFiles()
    {
        return luaConfiguration.Value.ScriptsFolders
            .SelectMany(folder =>
            {
                logger.LogDebug(LoadedScriptFolderLogMessage, folder);
                return directoryProxy.EnumerateFiles(
                    folder,
                    luaConfiguration.Value.LuaExtensionsPattern,
                    SearchOption.AllDirectories);
            })
            .ToList();
    }
}