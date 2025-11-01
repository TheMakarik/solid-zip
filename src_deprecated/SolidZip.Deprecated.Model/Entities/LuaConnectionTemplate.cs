namespace SolidZip.Deprecated.Model.Entities;

public record struct LuaConnectionTemplate(
    Func<string, object> GetLuaConnectionIndexer,
    Action<string, object> SetLuaIndexer,
    Action LuaConnectionDispose,
    Func<string, object[]>  DoFileTemplate,
    Func<string, object[]> DoStringTemplate
);