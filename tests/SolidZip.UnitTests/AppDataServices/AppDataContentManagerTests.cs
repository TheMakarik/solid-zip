namespace SolidZip.UnitTests.AppDataServices;

public class AppDataContentManagerTests
{
    private readonly ILogger<AppDataContentManager> _loggerStub = A.Dummy<ILogger<AppDataContentManager>>();

    [Theory]
    [AutoTestData]
    internal async Task ChangeThemeNameAsync_ShouldUpdateThemeName(
        AppDataContent initialContent,
        string newThemeName,
        string filePath)
    {
        // Arrange
        var serializerMock = A.Fake<IJsonSerializer>();
        var optionsMock = A.Fake<IOptions<AppDataOptions>>();
        
        optionsMock.FakeAppDataOptions(filePath);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .Returns(Task.FromResult(initialContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        await systemUnderTest.ChangeThemeNameAsync(newThemeName);

        // Assert
        A.CallTo(() => serializerMock.SerializeAsync(
                A<AppDataContent>.That.Matches(content => content.ThemeName == newThemeName),
                filePath,
                A<JsonSerializerOptions>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoTestData]
    internal async Task ChangeCurrentCultureAsync_ShouldUpdateCurrentCulture(
        AppDataContent initialContent,
        CultureInfo newCulture,
        string filePath)
    {
        // Arrange
        var serializerMock = A.Fake<IJsonSerializer>();
        var optionsMock = A.Fake<IOptions<AppDataOptions>>();
        
        optionsMock.FakeAppDataOptions(filePath);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .Returns(Task.FromResult(initialContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        await systemUnderTest.ChangeCurrentCultureAsync(newCulture);
        
        // Assert
        A.CallTo(() => serializerMock.SerializeAsync(
                A<AppDataContent>.That.Matches(content => content.CurrentCulture == newCulture),
                filePath,
                A<JsonSerializerOptions>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoTestData]
    internal async Task ChangeExplorerElementsViewAsync_ShouldUpdateExplorerElementsView(
        AppDataContent initialContent,
        ExplorerElementsView newView,
        string filePath)
    {
        // Arrange
        var serializerMock = A.Fake<IJsonSerializer>();
        var optionsMock = A.Fake<IOptions<AppDataOptions>>();
        
        optionsMock.FakeAppDataOptions(filePath);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .Returns(Task.FromResult(initialContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        await systemUnderTest.ChangeExplorerElementsViewAsync(newView);

        // Assert
        A.CallTo(() => serializerMock.SerializeAsync(
                A<AppDataContent>.That.Matches(content => content.ExplorerElementsView == newView),
                filePath,
                A<JsonSerializerOptions>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoTestData]
    internal async Task GetCurrentCultureAsync_ShouldReturnCurrentCulture(
        AppDataContent appDataContent,
        string filePath)
    {
        // Arrange
        var serializerMock = A.Fake<IJsonSerializer>();
        var optionsMock = A.Fake<IOptions<AppDataOptions>>();
        
        optionsMock.FakeAppDataOptions(filePath);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .Returns(Task.FromResult(appDataContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        var result = await systemUnderTest.GetCurrentCultureAsync();

        // Assert
        result.Should().Be(appDataContent.CurrentCulture);
        
        var result2 = await systemUnderTest.GetCurrentCultureAsync();
        result2.Should().Be(appDataContent.CurrentCulture);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoTestData]
    internal async Task GetExplorerElementsViewAsync_ShouldReturnExplorerElementsView(
        AppDataContent appDataContent,
        string filePath)
    {
        // Arrange
        var serializerMock = A.Fake<IJsonSerializer>();
        var optionsMock = A.Fake<IOptions<AppDataOptions>>();
        
        optionsMock.FakeAppDataOptions(filePath);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .Returns(Task.FromResult(appDataContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        var result = await systemUnderTest.GetExplorerElementsViewAsync();

        // Assert
        result.Should().Be(appDataContent.ExplorerElementsView);
        
        var result2 = await systemUnderTest.GetExplorerElementsViewAsync();
        result2.Should().Be(appDataContent.ExplorerElementsView);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoTestData]
    internal async Task GetThemeNameAsync_ShouldReturnThemeName(
        AppDataContent appDataContent,
        string filePath)
    {
        // Arrange
        var serializerMock = A.Fake<IJsonSerializer>();
        var optionsMock = A.Fake<IOptions<AppDataOptions>>();
        
        optionsMock.FakeAppDataOptions(filePath);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .Returns(Task.FromResult(appDataContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        var result = await systemUnderTest.GetThemeNameAsync();

        // Assert
        result.Should().Be(appDataContent.ThemeName);
        
        var result2 = await systemUnderTest.GetThemeNameAsync();
        result2.Should().Be(appDataContent.ThemeName);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoTestData]
    internal async Task GetMethods_ShouldUseCache_AfterFirstCall(
        AppDataContent appDataContent,
        string filePath)
    {
        // Arrange
        var serializerMock = A.Fake<IJsonSerializer>();
        var optionsMock = A.Fake<IOptions<AppDataOptions>>();
        
        optionsMock.FakeAppDataOptions(filePath);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .Returns(Task.FromResult(appDataContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act - first calls (should load from file)
        var culture1 = await systemUnderTest.GetCurrentCultureAsync();
        var theme1 = await systemUnderTest.GetThemeNameAsync();
        var view1 = await systemUnderTest.GetExplorerElementsViewAsync();

        // Act - second calls (should use cache)
        var culture2 = await systemUnderTest.GetCurrentCultureAsync();
        var theme2 = await systemUnderTest.GetThemeNameAsync();
        var view2 = await systemUnderTest.GetExplorerElementsViewAsync();

        // Assert
        culture1.Should().Be(appDataContent.CurrentCulture);
        theme1.Should().Be(appDataContent.ThemeName);
        view1.Should().Be(appDataContent.ExplorerElementsView);
        
        culture2.Should().Be(culture1);
        theme2.Should().Be(theme1);
        view2.Should().Be(view1);
        
        A.CallTo(() => serializerMock.DeserializeAsync<AppDataContent>(
                filePath, 
                A<JsonSerializerOptions>._))
            .MustHaveHappenedOnceExactly();
    }
    
}