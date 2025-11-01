namespace SolidZip.Model.Entities;

public readonly struct ReadonlyLuaTable<TRealTable>(ReadonlyLuaTableTemplate<TRealTable> template)
{
    public object this[string fullPath] => template.ReadValueFromLuaTable(fullPath, template);

    public ICollection GetDeeperLuaTableValues(string tableName)
    {
        return template.GetDeeperTableValues(tableName, template);
    }

    public void InvokeFunction(string functionName)
    {
        template.InvokeNoReturningLuaFunction(functionName, template);
    }
}