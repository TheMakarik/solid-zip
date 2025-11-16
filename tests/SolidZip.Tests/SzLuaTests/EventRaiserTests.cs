using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLua;
using SolidZip.Core.Common;
using SolidZip.Core.Contracts.LuaModules;
using SolidZip.Core.Extensions;
using SolidZip.Modules.LuaModules;

namespace SolidZip.Tests.SzLuaTests;

public class EventRaiserTests : IDisposable
{
    private readonly Lua _lua;
    private readonly ILuaEventRaiser _eventRaiser;
    private readonly LuaGlobalsLoader _globalsLoader;

    public EventRaiserTests()
    {
         var paths = A.Fake<PathsCollection>();
         A.CallTo(() => paths.Modules)
             .Returns(Consts.ModulesFolder);
         _eventRaiser = A.Dummy<ILuaEventRaiser>();
         var services = new ServiceCollection();
         services.AddSingleton(_eventRaiser);
         var serviceProvider = services.BuildServiceProvider();
         _globalsLoader = new LuaGlobalsLoader(
             loggerFactory: A.Dummy<ILoggerFactory>(),
             A.Dummy<ILogger<LuaGlobalsLoader>>(),
             serviceProvider,
             A.Dummy<ILuaDebugConsole>(),
             A.Dummy<ILuaShared>(),
             paths);
         _lua = new Lua();
        
    }
    
    [Theory]
    [InlineData("event_name")]
    public void LuaRaiseMethod_WithoutArgs_MustInvokeRaiseBackgroundInEventRaiser(string eventName)
    {
        //Arrange
        var scriptPath = Path.Combine(Consts.LuaScriptFolder, "event-raiser.lua");
        _globalsLoader.Load(_lua, scriptPath);
        //Act
        _lua["event_name"] = eventName;
        _lua.DoFile(scriptPath);
        //Assert
        A.CallTo(() => _eventRaiser.RaiseBackground(eventName))
            .MustHaveHappened();
    }
    
    public void Dispose()
    {
        _lua.Dispose();
    }
}