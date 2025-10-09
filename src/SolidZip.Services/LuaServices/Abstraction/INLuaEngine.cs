namespace SolidZip.Services.LuaServices.Abstraction;

public interface INLuaEngine
{
    public object[] Execute(string path);
}