namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaStateCaching
{
    public void Cache(Lua state, string path);
    public Lua? Get(string path);
    public void Uncache(Lua state, string path);
}