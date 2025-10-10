namespace SolidZip.Services.LuaServices;

internal sealed class LuaExtensions(ILogger<LuaExtensions> logger) : ILuaExtensions
{
    private const string LoadingEventLogMessage = "Loading {name} event";
    
    private FrozenDictionary<string, ImmutableArray<string>> _extensionsOnEvent;
    
    public IEnumerable<string> GetLuaExtensions(string eventName)
    {
        return _extensionsOnEvent.TryGetValue(eventName, out var extensions)
            ? extensions
            : [];
    }

    public void Load(ConcurrentDictionary<string, ImmutableArray<string>> extensions)
    {
        _extensionsOnEvent = extensions.ToFrozenDictionary();
    }
}