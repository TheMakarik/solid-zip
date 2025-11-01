namespace SolidZip.Deprecated.Model.Entities;

public record ReadonlyLuaTableTemplate<TRealTable>(
    Func<string, ReadonlyLuaTableTemplate<TRealTable>, object> ReadValueFromLuaTable,
    Func<string, ReadonlyLuaTableTemplate<TRealTable>, ICollection> GetDeeperTableValues,
    Action<string, ReadonlyLuaTableTemplate<TRealTable>> InvokeNoReturningLuaFunction,
    Func<object, TRealTable> ToRealLuaTable)
{
    public TRealTable RealLuaTable { get; set; }
}