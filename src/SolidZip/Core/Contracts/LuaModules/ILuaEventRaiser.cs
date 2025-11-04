namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaEventRaiser
{
    public ValueTask Raise(string @event);
    public ValueTask RaiseBackground(string @event);
    public ValueTask<T[]> Raise<T>(string @event);
}