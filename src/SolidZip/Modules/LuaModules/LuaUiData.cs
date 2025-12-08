namespace SolidZip.Modules.LuaModules;

public class LuaUiData(ILogger<LuaUiData> logger) : ILuaUiData
{
    private readonly ConcurrentDictionary<string, object?> _uiData = new();
    
    public object? Get(string name)
    {
        if (_uiData.TryGetValue(name, out var result))
            return result;
        
        
        logger.LogWarning("Key {key} not found in ui data", name);
        return null;
    }

    public void AddOrUpdate(string name, object?  value)
    {
        if(value is not null)
            logger.LogDebug("Updating {name} to {value} in ui table", name, value);
        
        _uiData.AddOrUpdate(name, value, (_, _) => value);
    }
}