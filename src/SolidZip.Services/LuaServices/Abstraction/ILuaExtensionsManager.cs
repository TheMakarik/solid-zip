using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace SolidZip.Services.LuaServices.Abstraction;

public interface ILuaExtensionsManager
{
    public IEnumerable<string> GetEventScripts(string eventName);
    public Task SubscribeAllAsync();
}