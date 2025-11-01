namespace SolidZip.Services.LuaServices.Abstraction;

public interface ILuaExtensionsRaiser
{
    public void RaiseBackground(string eventName);
    public Task RaiseAsync(string eventName);
    public IAsyncEnumerable<T> RaiseAsync<T>(string eventName);
}