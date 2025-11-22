using System.IO;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLua;
using SolidZip.Core.Common;
using SolidZip.Core.Contracts.LuaModules;
using SolidZip.Core.Utils;
using SolidZip.Modules.LuaModules;

namespace SolidZip.Tests.SzLuaTests;

public class IconsTests
{
    private readonly Lua _lua;
    private readonly AssociatedIconExtractor _associatedIconExtractor;
    private readonly ExtensionIconExtractor _extensionIconExtractor;
    private readonly LuaGlobalsLoader _globalsLoader;

    public IconsTests()
    { 
        var paths = A.Fake<PathsCollection>();
        A.CallTo(() => paths.Modules)
            .Returns(Consts.ModulesFolder);
        _associatedIconExtractor = A.Fake<AssociatedIconExtractor>();
        _extensionIconExtractor = A.Fake<ExtensionIconExtractor>();
        var services = new ServiceCollection();
        services
            .AddSingleton(_extensionIconExtractor)
            .AddSingleton(_associatedIconExtractor);
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
    [InlineData("C:\\")]
    public void LuaFromFileMethod_ExtractIcon(string testIconPath)
    {
        //Arrange
        var scriptPath = Path.Combine(Consts.LuaScriptFolder, "assoc-icon-extractor.lua");
        _globalsLoader.Load(_lua, scriptPath);
        _lua["test_path"] = testIconPath;
        //Act
        _lua.DoFile(scriptPath);
        //Assert
        A.CallTo(() => _associatedIconExtractor.Extract(testIconPath))
            .MustHaveHappened();
    }
    
    [Theory]
    [InlineData(".txt")]
    public void LuaFromExtensionMethod_ExtractIcon(string testExtension)
    {
        //Arrange
        var scriptPath = Path.Combine(Consts.LuaScriptFolder, "extension-icon-extractor.lua");
        _globalsLoader.Load(_lua, scriptPath);
        _lua["test_ext"] = testExtension;
        //Act
        _lua.DoFile(scriptPath);
        //Assert
        A.CallTo(() => _extensionIconExtractor.Extract(testExtension))
            .MustHaveHappened();
    }
}