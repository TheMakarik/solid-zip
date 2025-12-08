using System.ComponentModel;
using System.IO;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLua;
using SolidZip.Core.Common;
using SolidZip.Core.Contracts.LuaModules;
using SolidZip.Core.Extensions;
using SolidZip.Modules.LuaModules;
using SolidZip.Modules.LuaModules.LuaUtils;

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
             A.Dummy<ILuaUiData>(),
             A.Dummy<LuaEventRedirector>(),
             A.Dummy<MaterialIconLuaLoader>(),
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
    
    [Theory]
    [InlineData("event_name", "testarg")]
    public void LuaRaiseMethod_WithArgs_MustInvokeRaiseBackgroundInEventRaiser(string eventName, object args)
    {
        //Arrange
        var scriptPath = Path.Combine(Consts.LuaScriptFolder, "event-raiser-withargs.lua");
        _globalsLoader.Load(_lua, scriptPath);
        //Act
        _lua["event_name"] = eventName;
        _lua["event_args"] = args;
        _lua.DoFile(scriptPath);
        //Assert
        A.CallTo(() => _eventRaiser.RaiseBackground<string>(eventName, args.ToString()))
            .MustHaveHappened();
    }
    
    public void Dispose()
    {
        _lua.Dispose();
    }
}