using SolidZip.Services.LuaServices.Abstraction;

namespace SolidZip.Services.LuaServices;

public class LuaExtensionsRaiser(ILogger<LuaExtensionsRaiser> logger, ILuaScriptExecutor scriptExecutor, ILuaExtensionsManager extensionsManager) : ILuaExtensionsRaiser
{
    private const string RaisingEventLogMessage = "Raising {name} event..";
    
    public void RaiseBackground(string eventName)
    {
        Task.Factory.StartNew(() =>
        {
            logger.LogDebug(RaisingEventLogMessage, eventName);
            foreach (var file in extensionsManager.GetEventScripts(eventName))
                scriptExecutor.ExecuteAsync(file);
        }, TaskCreationOptions.LongRunning);

    }

    public async ValueTask RaiseAsync(string eventName)
    {
        logger.LogDebug(RaisingEventLogMessage, eventName);
    }

    public async ValueTask<T> RaiseAsync<T>(string eventName)
    {
        logger.LogDebug(RaisingEventLogMessage, eventName);
        throw new NotImplementedException();
    }
}