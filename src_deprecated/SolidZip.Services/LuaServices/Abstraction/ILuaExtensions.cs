namespace SolidZip.Services.LuaServices.Abstraction;

public interface ILuaExtensions
{
    public IEnumerable<string> GetLuaExtensions(string eventName);
    public void Load(ConcurrentDictionary<string, ImmutableArray<string>> extensions);
}