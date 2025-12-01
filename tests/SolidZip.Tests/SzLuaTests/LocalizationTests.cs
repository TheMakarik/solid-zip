using System.Globalization;
using System.IO;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLua;
using SolidZip.Core.Common;
using SolidZip.Core.Contracts.AppData;
using SolidZip.Core.Contracts.LuaModules;
using SolidZip.Modules.LuaModules;

namespace SolidZip.Tests.SzLuaTests;

public class LocalizationTests : IDisposable
{
    private readonly Lua _lua;
    private readonly LuaGlobalsLoader _globalsLoader;
    private readonly IUserJsonManager _userJsonManager;

    public LocalizationTests()
    {
        var paths = A.Fake<PathsCollection>();
        A.CallTo(() => paths.Modules)
            .Returns(Consts.ModulesFolder);
        var services = new ServiceCollection();
        _userJsonManager = A.Fake<IUserJsonManager>();
        services.AddSingleton(_userJsonManager);
        var serviceProvider = services.BuildServiceProvider();
        _globalsLoader = new LuaGlobalsLoader(
            loggerFactory: A.Dummy<ILoggerFactory>(),
            A.Dummy<ILogger<LuaGlobalsLoader>>(),
            serviceProvider,
            A.Dummy<ILuaDebugConsole>(),
            A.Dummy<ILuaShared>(),
            A.Dummy<ILuaUiData>(),
            paths);
        _lua = new Lua();
        
    }

    [Theory]
    [InlineData("ru-RU")]
    public void LuaChangeLoc_MustInvokeChangeCultureInfoFromUserJsonManager(string culture)
    {
        //Arrange
        var scriptPath = Path.Combine(Consts.LuaScriptFolder, "loc-changeloc.lua");
        _globalsLoader.Load(_lua, scriptPath);
        _lua["culture"] = culture;
        
        //Act
        _lua.DoFile(scriptPath);
        //Assert
        A.CallTo(() => _userJsonManager.ChangeCurrentCulture(new CultureInfo(culture)))
            .MustHaveHappened();
    }
    
    public void Dispose()
    {
        _lua.Dispose();
    }
}