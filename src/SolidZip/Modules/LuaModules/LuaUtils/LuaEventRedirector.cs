namespace SolidZip.Modules.LuaModules.LuaUtils;

public class LuaEventRedirector(ILogger<LuaEventRedirector> logger)
{
    public void RedirectEvent(object eventOwner, string eventName, string luaEventName, ILuaEventRaiser luaEventRaiser)
    {
        var eventInfo = eventOwner.GetType().GetEvent(eventName)!;
        var delegateType = eventInfo.EventHandlerType;
        
        EventHandler handler = (sender, args) => 
        {
            _ = luaEventRaiser.RaiseAsync(luaEventName);
        };
        
        var convertedHandler = Delegate.CreateDelegate(
            delegateType, 
            handler.Target!, 
            handler.Method);
        
        logger.LogDebug("Redirect .NET event {dotnetEvent} to szlua event: {luaEvent}", eventName, luaEventName);
        
        eventInfo.AddEventHandler(eventOwner, convertedHandler);
    }
}