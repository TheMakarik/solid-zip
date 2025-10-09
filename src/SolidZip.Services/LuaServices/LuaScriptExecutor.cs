using SolidZip.Services.LuaServices.Abstraction;

namespace SolidZip.Services.LuaServices;

internal sealed class LuaScriptExecutor(INLuaEngine nLuaEngine, IOptions<LuaConfiguration> luaConfiguration) : ILuaScriptExecutor
{
    public async Task<T> ExecuteAsync<T>(string path)
    {
        var result = await LoadFunction<Func<T>>(path);
        return await Task.Run(result);
    }

    public async Task ExecuteAsync(string path)
    {
        var result = await LoadFunction<Action>(path);
        await Task.Run(result);
    }

    private async Task<T> LoadFunction<T>(string path) where T : Delegate
    {
        var result = await Task.Run(() =>
        {
            using var table =  nLuaEngine.Execute(path).FirstOrDefault() as LuaTable;
            var action = (T)table?[luaConfiguration.Value.ExecuteFunctionName]!;
            return action;
        });
        return result;
    }
}