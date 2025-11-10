namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaEventRaiser
{
    public ValueTask Raise<T>(string @event, T args);
    public ValueTask RaiseBackground<T>(string @event, T args);
    public ValueTask<TReturn[]> Raise<TReturn, TArgs>(string @event, TArgs value);
    public ValueTask Raise(string @event);
    public ValueTask  RaiseBackground(string @event);
    public ValueTask<TReturn[]> Raise<TReturn>(string @event);
}