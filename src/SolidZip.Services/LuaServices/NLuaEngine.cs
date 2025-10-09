using SolidZip.Services.LuaServices.Abstraction;

namespace SolidZip.Services.LuaServices;

public sealed class NLuaEngine(
    ILuaGlobalsLoader globalsLoader
    ) : INLuaEngine
{
    public object[] Execute(string scriptPath)
    {
        using var lua = new Lua();
        globalsLoader.Load(lua, scriptPath);
        return lua.DoFile(scriptPath);
    }

    
}