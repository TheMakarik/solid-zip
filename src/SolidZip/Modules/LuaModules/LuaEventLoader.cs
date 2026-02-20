namespace SolidZip.Modules.LuaModules;

public sealed class LuaEventLoader(
    ILogger<LuaEventLoader> logger,
    PathsCollection paths,
    ILuaDebugConsole console,
    ILuaEvents events) : ILuaEventLoader
{
    private readonly ConcurrentDictionary<string, ImmutableArray<string>> _eventOnExtensions = new();
    private IProgress<double> _progress;
    private double _progressStep;


    public async Task LoadAsync(IProgress<double> progress, double progressMaxAdd)
    {
        logger.LogDebug("Start searching lua-extensions");
        _progress = progress;
        _progressStep = progressMaxAdd / paths.Plugins.Length;
        var stopwatch = Stopwatch.StartNew();
        var count = await LoadWithoutTimerAsync();
        logger.LogInformation("Found {count} extensions for {time}ms", count, stopwatch.ElapsedMilliseconds);
        stopwatch.Stop();
    }

    private async Task<int> LoadWithoutTimerAsync()
    {
        var counter = 0;
        await Parallel.ForEachAsync(paths.Plugins, async (pluginsDirectory, token) =>
        {
            if (!Directory.Exists(pluginsDirectory))
            {
                Directory.CreateDirectory(pluginsDirectory);
                logger.LogInformation("Lua plugins directory {path} was unexisted, so was created", pluginsDirectory);
                return;
            }

            var extensions = EnumerateLuaExtensions(pluginsDirectory);

            foreach (var extension in extensions)
            {
                logger.LogDebug("Handling extension file: {extension}", extension);
                counter++;
                await AddLuaExtensionAsync(extension);
            }

            _progress.Report(_progressStep);
        });
        events.Register(_eventOnExtensions);
        return counter;
    }

    private Task AddLuaExtensionAsync(string extension)
    {
        return Task.Factory.StartNew(() =>
        {
            using var lua = new Lua();
            LoadLua(lua, extension);
            var eventsTable = lua.GetTable("script")["events"] as LuaTable;
            var events = eventsTable?.Values.Cast<string>().ToArray() ?? [];
            logger.LogInformation("{count} events were found at {extension}", events.Length, extension);
            foreach (var @event in events)
                RegisterEventOnExtensions(@event, extension);
        }, TaskCreationOptions.LongRunning);
    }

    private void LoadLua(Lua lua, string extension)
    {
        try
        {
            lua.DoString("script = {}");
            lua.DoString("_ENV = {};");
            lua.DoString("_G = {};");
            lua.DoFile(extension);
        }
        catch (Exception exception)
        {
            var exceptionMessage = $"Exception occured: {exception.Message}";
            console.PrintAsync(exceptionMessage, extension, ConsoleColor.Red);
            logger.LogError("{message} at path {path}", exceptionMessage, extension);
        }
    }

    private void RegisterEventOnExtensions(string @event, string extension)
    {
        _eventOnExtensions.AddOrUpdate(@event, [extension],
            (_, oldValue) => oldValue.Add(extension));
    }

    private IEnumerable<string> EnumerateLuaExtensions(string pluginsDirectory)
    {
        return Directory.EnumerateFiles(
            pluginsDirectory, "ext-*.lua",
            new EnumerationOptions { RecurseSubdirectories = true });
    }
}