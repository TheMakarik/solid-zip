namespace SolidZip.Services.LuaServices.Abstraction;

public interface ILuaScriptExecutor
{
    public Task<T> ExecuteAsync<T>(string path);
    public Task ExecuteAsync(string path);
}