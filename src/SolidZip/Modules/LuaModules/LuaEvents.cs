namespace SolidZip.Modules.LuaModules;

public class LuaEvents(ILogger<LuaEvents> logger) : ILuaEvents
{
    private ConcurrentDictionary<string, ImmutableArray<string>> _events = new();
    
    public void Register(ConcurrentDictionary<string, ImmutableArray<string>> events)
    {
        _events = events;
        logger.LogDebug("Loaded lua events: {events}", events.Keys);
    }

    public string[] Get(string @event)
    {
        return _events.TryGetValue(@event, out var result) 
            ? result.ToArray() 
            : [];
    }
}