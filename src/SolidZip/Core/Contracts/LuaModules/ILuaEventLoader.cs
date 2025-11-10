namespace SolidZip.Core.Contracts.LuaModules;

public interface ILuaEventLoader
{
    public Task LoadAsync(IProgress<double> progress, double progressMaxAdd);
}