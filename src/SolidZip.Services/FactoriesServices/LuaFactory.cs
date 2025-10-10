namespace SolidZip.Services.FactoriesServices;

internal sealed class LuaFactory(IOptions<LuaConfiguration> luaConfiguration, ILuaGlobalsLoader globalsLoader) : ILuaFactory<LuaTable>
{
    public LuaConnection<LuaTable> GetFactory(string pathForLogging)
    {
        var lua = new Lua();
        lua.LoadCLRPackage();
        lua.State.Encoding = Encoding.UTF8;

        var template = CreateTemplate(lua);
        var tableTemplate = CreateTableTemplate();
        var connection = new LuaConnection<LuaTable>(template, tableTemplate);
        globalsLoader.Load(connection, pathForLogging);
        return connection;
    }

    private LuaConnectionTemplate CreateTemplate(Lua lua)
    {
        var template = new LuaConnectionTemplate(
            GetLuaConnectionIndexer: fullPath => lua[fullPath],
            SetLuaIndexer: (fullPath, value) => lua[fullPath] = value,
            DoFileTemplate: lua.DoFile,
            DoStringTemplate: (script) => lua.DoString(script),
            LuaConnectionDispose: lua.Dispose
        );
        return template;
    }

    private ReadonlyLuaTableTemplate<LuaTable> CreateTableTemplate()
    {
        var tableTemplate = new ReadonlyLuaTableTemplate<LuaTable>(
            ToRealLuaTable: @object => (LuaTable)@object,
            ReadValueFromLuaTable: (fullPath, self) => self.RealLuaTable[fullPath],
            InvokeNoReturningLuaFunction: (function, self) => (self.RealLuaTable[function] as LuaFunction).Call(),
            GetDeeperTableValues: (table, self) => (self.RealLuaTable[table] as LuaTable).Values
        );
        return tableTemplate;
    }
    
    
}