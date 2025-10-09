namespace SolidZip.Services.LuaServices.Abstraction;

public interface ILuaExtensionsRaiser
{
    public void RaiseBackground(string eventName);
    public ValueTask RaiseAsync(string eventName);
    public ValueTask<T> RaiseAsync<T>(string eventName);
}