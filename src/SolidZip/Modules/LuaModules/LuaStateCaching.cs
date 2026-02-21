namespace SolidZip.Modules.LuaModules;

public class LuaStateCaching(ILogger<LuaStateCaching> logger) : ILuaStateCaching
{
    private ConcurrentDictionary<string, Lua> _states = new();
    
    public void Cache(Lua state, string path)
    {
        if (!_states.ContainsKey(path))
        {
            var result = _states.TryAdd(path, state);
            if (result)
                logger.LogInformation("State at path {path} was added", path);
            else
                logger.LogError("State at path {path} was not added", path);
        }
        else
            logger.LogInformation("State at path {path}  is already added", path);
        
    }

    public Lua? Get(string path)
    {
        return _states.TryGetValue(path, out var state) ? state : null;
    }

    public void Uncache(Lua state, string path)
    {
        if (!_states.ContainsKey(path))
        {
            logger.LogError("State at path {path} was not added, so cannot remove it", path);
        }
        
        state.DoString(@"
        if script.end ~= nil then
            script.end();
        end
        ");
        var result = _states.TryRemove(path, out _);
        if (result)
            logger.LogInformation("State at path {path} was removed", path);
        else
            logger.LogError("State at path {path} was not removed", path);
    }
}