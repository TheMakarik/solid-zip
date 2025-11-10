namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaEvents
{
    public void Register(ConcurrentDictionary<string, ImmutableArray<string>> events);
    public string[] Get(string @event);
}