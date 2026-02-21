using System.Globalization;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLua;
using SolidZip.Core.Common;
using SolidZip.Core.Contracts.AppData;
using SolidZip.Core.Contracts.LuaModules;
using SolidZip.Modules.LuaModules;
using SolidZip.Modules.LuaModules.LuaUtils;

namespace SolidZip.Testing.SzLuaTests;

public class LocalizationTests : IDisposable
{
    private readonly LuaGlobalsLoader _globalsLoader;
    private readonly Lua _lua;
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
            A.Dummy<ILoggerFactory>(),
            A.Dummy<ILogger<LuaGlobalsLoader>>(),
            serviceProvider,
            A.Dummy<ILuaDebugConsole>(),
            A.Dummy<ILuaShared>(),
            A.Dummy<ILuaUiData>(),
            A.Dummy<LuaEventRedirector>(),
            A.Dummy<MaterialIconLuaLoader>(),
            A.Dummy<ILuaStateCaching>(),
            paths);
        _lua = new Lua();
    }

    public void Dispose()
    {
        _lua.Dispose();
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


    [Fact]
    public void LuaCurrent_MustReturnCurrentUICulture()
    {
        //Arrange
        var scriptPath = Path.Combine(Consts.LuaScriptFolder, "loc-getcurrent.lua");
        _globalsLoader.Load(_lua, scriptPath);

        //Act
        _lua.DoFile(scriptPath);

        //Assert
        _lua["result"].Should().BeOfType<string>().And.Be(CultureInfo.CurrentUICulture.ToString());
    }
}