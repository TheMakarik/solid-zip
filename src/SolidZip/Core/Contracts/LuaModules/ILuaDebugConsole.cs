namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaDebugConsole
{
    public ValueTask AttachAsync();
    public Task PrintAsync(string text, string scriptPath);
}