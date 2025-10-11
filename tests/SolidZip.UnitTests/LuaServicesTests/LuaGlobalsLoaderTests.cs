using System.Text;

namespace SolidZip.UnitTests.LuaServicesTests;


public class LuaGlobalsLoaderTests
{
    private readonly ILogger<LuaGlobalsLoader> _loggerStub = A.Dummy<ILogger<LuaGlobalsLoader>>();
    private readonly ILoggerFactory _loggerFactoryMock = A.Fake<ILoggerFactory>();
    private readonly IDirectoryProxy _directoryProxyMock = A.Fake<IDirectoryProxy>();
    private readonly IOptions<LuaConfiguration> _luaConfigurationMock = A.Fake<IOptions<LuaConfiguration>>();
    private readonly IServiceProvider _serviceProviderMock = A.Fake<IServiceProvider>();

    private LuaConfiguration CreateLuaConfiguration(string[] modules = null)
    {
        return new LuaConfiguration 
        {
            ScriptsFolders = new[] { "Scripts/Test" },
            Modules = modules ?? [],
            LuaExtensionsPattern = "*.lua",
            ExecuteFunctionName = "execute",
            EventsField = "events",
            LuaScriptName = "script"
        };
    }

    [Theory]
    [AutoTestData]
    internal void Load_WithExistingModulesFolder_AddsPathsToPackagePath(
        LuaConnection<object> luaMock,
        string scriptPath,
        string[] modulesFolders)
    {
        // Arrange
        var configuration = CreateLuaConfiguration(modulesFolders);
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);
        
        foreach (var folder in modulesFolders)
        {
            A.CallTo(() => _directoryProxyMock.Exists(folder)).Returns(true);
        }

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        systemUnderTest.Load(luaMock, scriptPath);

        // Assert
        A.CallTo(() => luaMock.DoString(A<string>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoTestData]
    internal void Load_WithNonExistingModulesFolder_LogsWarning(
        LuaConnection<object> luaMock,
        string scriptPath,
        string[] modulesFolders)
    {
        // Arrange
        var configuration = CreateLuaConfiguration(modulesFolders);
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);
        
        foreach (var folder in modulesFolders)
        {
            A.CallTo(() => _directoryProxyMock.Exists(folder)).Returns(false);
        }

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        systemUnderTest.Load(luaMock, scriptPath);

        // Assert
        A.CallTo(() => _directoryProxyMock.Exists(A<string>._))
            .MustHaveHappened(modulesFolders.Length, Times.Exactly);
    }

    [Theory]
    [AutoTestData]
    internal void Load_WithMixedModulesFolders_AddsOnlyExistingPaths(
        LuaConnection<object> luaMock,
        string scriptPath,
        string[] existingFolders,
        string[] nonExistingFolders)
    {
        // Arrange
        var allFolders = existingFolders.Concat(nonExistingFolders).ToArray();
        var configuration = CreateLuaConfiguration(allFolders);
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);
        
        foreach (var folder in existingFolders)
        {
            A.CallTo(() => _directoryProxyMock.Exists(folder)).Returns(true);
        }
        
        foreach (var folder in nonExistingFolders)
        {
            A.CallTo(() => _directoryProxyMock.Exists(folder)).Returns(false);
        }

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        systemUnderTest.Load(luaMock, scriptPath);

        // Assert
        A.CallTo(() => _directoryProxyMock.Exists(A<string>._))
            .MustHaveHappened(allFolders.Length, Times.Exactly);
    }

    [Theory]
    [AutoTestData]
    internal void Load_WithEmptyModules_DoesNotAddPaths(
        LuaConnection<object> luaMock,
        string scriptPath)
    {
        // Arrange
        var configuration = CreateLuaConfiguration([]);
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        systemUnderTest.Load(luaMock, scriptPath);

        // Assert
        A.CallTo(() => _directoryProxyMock.Exists(A<string>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoTestData]
    internal void Load_WithNullModules_DoesNotAddPaths(
        LuaConnection<object> luaMock,
        string scriptPath)
    {
        // Arrange
        var configuration = CreateLuaConfiguration(null);
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        systemUnderTest.Load(luaMock, scriptPath);

        // Assert
        A.CallTo(() => _directoryProxyMock.Exists(A<string>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoTestData]
    internal void Load_SetsFromSzFunction(
        LuaConnection<object> luaMock,
        string scriptPath)
    {
        // Arrange
        var configuration = CreateLuaConfiguration(Array.Empty<string>());
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        systemUnderTest.Load(luaMock, scriptPath);

        // Assert
        A.CallTo(() => luaMock.SetIndexer("fromSz", A<Func<string, object>>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoTestData]
    internal void Load_CreatesLoggerForScriptPath(
        LuaConnection<object> luaMock,
        string scriptPath)
    {
        // Arrange
        var configuration = CreateLuaConfiguration(Array.Empty<string>());
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);
        
        var scriptLoggerMock = A.Dummy<ILogger>();
        A.CallTo(() => _loggerFactoryMock.CreateLogger(scriptPath))
            .Returns(scriptLoggerMock);

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        systemUnderTest.Load(luaMock, scriptPath);

        // Assert
        A.CallTo(() => _loggerFactoryMock.CreateLogger(scriptPath))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoTestData]
    internal void LoadModules_WithExistingFolders_ReturnsPathBuilderWithNormalizedPaths(
        string[] modulesFolders)
    {
        // Arrange
        var configuration = CreateLuaConfiguration(modulesFolders);
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);
        
        foreach (var folder in modulesFolders)
        {
            A.CallTo(() => _directoryProxyMock.Exists(folder)).Returns(true);
        }

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        var result = InvokePrivateMethod<StringBuilder>(systemUnderTest, "LoadModules", A.Dummy<LuaConnection<object>>());

        // Assert
        result.ToString().Should().NotBeEmpty();
        foreach (var folder in modulesFolders)
        {
            var normalizedPath = folder.Replace("\\", "/");
            result.ToString().Should().Contain(normalizedPath);
        }
    }

    [Theory]
    [AutoTestData]
    internal void LoadModules_WithNonExistingFolders_ReturnsPathBuilderWithoutThosePaths(
        string[] existingFolders,
        string[] nonExistingFolders)
    {
        // Arrange
        var allFolders = existingFolders.Concat(nonExistingFolders).ToArray();
        var configuration = CreateLuaConfiguration(allFolders);
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);
        
        foreach (var folder in existingFolders)
        {
            A.CallTo(() => _directoryProxyMock.Exists(folder)).Returns(true);
        }
        
        foreach (var folder in nonExistingFolders)
        {
            A.CallTo(() => _directoryProxyMock.Exists(folder)).Returns(false);
        }

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        var result = InvokePrivateMethod<StringBuilder>(systemUnderTest, "LoadModules", A.Dummy<LuaConnection<object>>());

        // Assert
        foreach (var folder in existingFolders)
        {
            var normalizedPath = folder.Replace("\\", "/");
            result.ToString().Should().Contain(normalizedPath);
        }
        
        foreach (var folder in nonExistingFolders)
        {
            var normalizedPath = folder.Replace("\\", "/");
            result.ToString().Should().NotContain(normalizedPath);
        }
    }

    [Theory]
    [AutoTestData]
    internal void GetServiceByTypeName_WithValidTypeName_ReturnsService(
        string typeName,
        object expectedService)
    {
        // Arrange
        var configuration = CreateLuaConfiguration(Array.Empty<string>());
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);
        
        var type = expectedService.GetType();
        A.CallTo(() => _serviceProviderMock.GetService(type))
            .Returns(expectedService);

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        var result = InvokePrivateMethod<object>(systemUnderTest, "GetServiceByTypeName", typeName);

        // Assert
        result.Should().Be(expectedService);
    }

    [Theory]
    [AutoTestData]
    internal void GetServiceByTypeName_WithNonExistentType_ThrowsInvalidOperationException(
        string typeName)
    {
        // Arrange
        var configuration = CreateLuaConfiguration(Array.Empty<string>());
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act & Assert
        Invoking(() => InvokePrivateMethod<object>(systemUnderTest, "GetServiceByTypeName", typeName))
            .Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [AutoTestData]
    internal void GetServiceByTypeName_WithUnregisteredService_ThrowsInvalidOperationException(
        Type serviceType)
    {
        // Arrange
        var configuration = CreateLuaConfiguration(Array.Empty<string>());
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);
        
        A.CallTo(() => _serviceProviderMock.GetService(serviceType))
            .Returns(null);

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act & Assert
        Invoking(() => InvokePrivateMethod<object>(systemUnderTest, "GetServiceByTypeName", serviceType.FullName))
            .Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [AutoTestData]
    internal void LoadLogger_SetsAllLoggingFunctions(
        LuaConnection<object> luaMock,
        string scriptPath)
    {
        // Arrange
        var configuration = CreateLuaConfiguration(Array.Empty<string>());
        A.CallTo(() => _luaConfigurationMock.Value).Returns(configuration);
        
        var scriptLoggerMock = A.Dummy<ILogger>();
        A.CallTo(() => _loggerFactoryMock.CreateLogger(scriptPath))
            .Returns(scriptLoggerMock);

        var systemUnderTest = new LuaGlobalsLoader(
            _loggerStub, 
            _loggerFactoryMock, 
            _directoryProxyMock, 
            _luaConfigurationMock, 
            _serviceProviderMock);

        // Act
        InvokePrivateMethod<object>(systemUnderTest, "LoadLogger", luaMock, scriptPath);

        // Assert
        A.CallTo(() => luaMock.SetIndexer("_info", A<Action<string>>._)).MustHaveHappened();
        A.CallTo(() => luaMock.SetIndexer("_debug", A<Action<string>>._)).MustHaveHappened();
        A.CallTo(() => luaMock.SetIndexer("_trace", A<Action<string>>._)).MustHaveHappened();
        A.CallTo(() => luaMock.SetIndexer("_warn", A<Action<string>>._)).MustHaveHappened();
        A.CallTo(() => luaMock.SetIndexer("_error", A<Action<string>>._)).MustHaveHappened();
        A.CallTo(() => luaMock.SetIndexer("_critical", A<Action<string>>._)).MustHaveHappened();
    }

    private static TReturn InvokePrivateMethod<TReturn>(object obj, string methodName, params object[] parameters)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (method == null)
            throw new ArgumentException($"Method '{methodName}' not found");
            
        return (TReturn)method.Invoke(obj, parameters);
    }

    private static Action Invoking(Action action) => () => action();
}