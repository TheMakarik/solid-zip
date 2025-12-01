namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaUiData
{
    public object? Get(string name);
    public void AddOrUpdate(string name, object value);
}