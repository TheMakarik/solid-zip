
namespace SolidZip.Modules.LuaModules;

public sealed class LuaUiData(ILogger<LuaUiData> logger) : ILuaUiData
{
    private readonly ConcurrentDictionary<string, object?> _uiData = new();

    public object? Get(string name)
    {
        return _uiData.TryGetValue(name, out var result) ? result : null;
    }

    public void AddOrUpdate(string name, object? value)
    {
        _uiData.AddOrUpdate(name, value, (_, _) => value);
    }
}