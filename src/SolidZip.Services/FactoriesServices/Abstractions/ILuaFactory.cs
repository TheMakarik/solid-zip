namespace SolidZip.Services.FactoriesServices.Abstractions;

public interface ILuaFactory<TRealLuaTable>
{
    public LuaConnection<TRealLuaTable> GetFactory(string pathForLogging);
}