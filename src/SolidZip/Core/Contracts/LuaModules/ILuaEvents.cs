namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaEvents
{
    public void Add(string @event, string path);
}