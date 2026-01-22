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

    public void AddOrUpdate(string name, object? value)
    {
        _uiData.AddOrUpdate(name, value, (_, _) => value);
    }
}