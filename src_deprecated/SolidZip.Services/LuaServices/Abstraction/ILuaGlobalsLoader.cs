
namespace SolidZip.Services.LuaServices.Abstraction;

public interface ILuaGlobalsLoader
{
    public void Load<T>(LuaConnection<T> lua, string path);
}