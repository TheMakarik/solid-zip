
namespace SolidZip.Services.LuaServices.Abstraction;

public interface ILuaGlobalsLoader
{
    public void Load(Lua lua, string path);
}