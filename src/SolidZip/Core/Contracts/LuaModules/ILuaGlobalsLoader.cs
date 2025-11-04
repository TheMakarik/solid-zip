namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaGlobalsLoader
{
    public void Load(Lua lua, string scriptPath);
}