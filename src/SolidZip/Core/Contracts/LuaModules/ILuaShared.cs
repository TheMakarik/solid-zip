namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaShared
{
    public object? Get(string name);
    public void Add(string name, object value);
}