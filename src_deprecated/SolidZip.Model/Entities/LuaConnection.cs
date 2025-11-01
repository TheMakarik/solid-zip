namespace SolidZip.Model.Entities;

public struct LuaConnection<TRealTable>(
    LuaConnectionTemplate connectionTemplate, 
    ReadonlyLuaTableTemplate<TRealTable> luaTableTemplate) : IDisposable
{
    public ReadonlyLuaTable<TRealTable> this[string fullPath]
    {
        get
        {
            var value = connectionTemplate.GetLuaConnectionIndexer(fullPath);
            luaTableTemplate.RealLuaTable = luaTableTemplate.ToRealLuaTable(value);
            return new ReadonlyLuaTable<TRealTable>(luaTableTemplate);
        }
    }

    public void SetIndexer(string fullPath, object value) => connectionTemplate.SetLuaIndexer(fullPath, value);

    public object[] DoFile(string path)
    {
        return connectionTemplate.DoFileTemplate(path);
    }

    public object DoString(string script)
    {
        return connectionTemplate.DoStringTemplate(script);
    }
    
    
    public void Dispose()
    {
        connectionTemplate.LuaConnectionDispose();
    }
}