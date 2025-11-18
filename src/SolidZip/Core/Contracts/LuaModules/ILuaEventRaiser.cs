namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaEventRaiser
{
    public ValueTask RaiseAsync<T>(string @event, T args);
    public ValueTask RaiseBackground<T>(string @event, T args);
    public ValueTask<TReturn[]> RaiseAsync<TReturn, TArgs>(string @event, TArgs value);
    public ValueTask RaiseAsync(string @event);
    public ValueTask  RaiseBackground(string @event);
    public ValueTask<TReturn[]> RaiseAsync<TReturn>(string @event);
}