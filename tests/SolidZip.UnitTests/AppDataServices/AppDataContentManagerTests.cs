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
        
        optionsMock.FakeFromShared(filePath);
        
        A.CallTo(serializerMock)
            .WithReturnType<Task<AppDataContent>>()
            .Where(fake => fake.Arguments.First() as string == filePath && fake.Method.Name == nameof(serializerMock.DeserializeAsync))
            .Returns(Task.FromResult(initialContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        await systemUnderTest.ChangeThemeNameAsync(newThemeName);

        // Assert
        A.CallTo(serializerMock)
            .Where(fake => 
                fake.Method.Name == nameof(serializerMock.SerializeAsync) 
                    && ((fake.Arguments.FirstOrDefault() as AppDataContent?)!)
                      .Value.ThemeName == newThemeName)
            .MustHaveHappened();
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
        
        optionsMock.FakeFromShared(filePath);
        
        A.CallTo(serializerMock)
            .WithReturnType<Task<AppDataContent>>()
            .Where(fake => 
                (fake.Arguments.First() as string) == filePath
                && fake.Method.Name == nameof(serializerMock.DeserializeAsync))
            .Returns(Task.FromResult(initialContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        await systemUnderTest.ChangeCurrentCultureAsync(newCulture);
        
        // Assert
        A.CallTo(serializerMock)
            .Where(fake =>
                fake.Method.Name == nameof(serializerMock.SerializeAsync) 
                && ((fake.Arguments.FirstOrDefault() as AppDataContent?)!)
                    .Value.CurrentCulture == newCulture
            )
            .MustHaveHappened();
    }

    [Theory]
    [AutoTestData]
    internal async Task ChangeExplorerElementsView_ShouldUpdateExplorerElementsView(
        AppDataContent initialContent,
        ExplorerElementsView newView,
        string filePath)
    {
        // Arrange
        var serializerMock = A.Fake<IJsonSerializer>();
        var optionsMock = A.Fake<IOptions<AppDataOptions>>();
        
        optionsMock.FakeFromShared(filePath);
        
        A.CallTo(serializerMock)
            .WithReturnType<Task<AppDataContent>>()
            .Where(fake => 
                (fake.Arguments.First() as string) == filePath
                && fake.Method.Name == nameof(serializerMock.DeserializeAsync))
            .Returns(Task.FromResult(initialContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        await systemUnderTest.ChangeExplorerElementsView(newView);

        // Assert
        A.CallTo(serializerMock)
            .WithReturnType<Task>()
            .Where(fake =>
                fake.Method.Name == nameof(serializerMock.SerializeAsync) 
                && ((fake.Arguments.FirstOrDefault() as AppDataContent?)!)
                .Value.ExplorerElementsView == newView
            )
            .MustHaveHappened();
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
        
        optionsMock.FakeFromShared(filePath);
        
        A.CallTo(serializerMock)
            .WithReturnType<Task<AppDataContent>>()
            .Where(fake => 
                (fake.Arguments.First() as string) == filePath
                && fake.Method.Name == nameof(serializerMock.DeserializeAsync))
            .Returns(Task.FromResult(appDataContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        var result = await systemUnderTest.GetCurrentCultureAsync();

        // Assert
        result.Should().Be(appDataContent.CurrentCulture);
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
        
        optionsMock.FakeFromShared(filePath);
        
        A.CallTo(serializerMock)
            .WithReturnType<Task<AppDataContent>>()
            .Where(fake => 
                (fake.Arguments.First() as string) == filePath
                && fake.Method.Name == nameof(serializerMock.DeserializeAsync))
            .Returns(Task.FromResult(appDataContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        var result = await systemUnderTest.GetExplorerElementsViewAsync();

        // Assert
        result.Should().Be(appDataContent.ExplorerElementsView);
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
        
        optionsMock.FakeFromShared(filePath);
        
        A.CallTo(serializerMock)
            .WithReturnType<Task<AppDataContent>>()
            .Where(fake => 
                (fake.Arguments.First() as string) == filePath
                && fake.Method.Name == nameof(serializerMock.DeserializeAsync))
            .Returns(Task.FromResult(appDataContent));
        
        var systemUnderTest = new AppDataContentManager(_loggerStub, serializerMock, optionsMock);

        // Act
        var result = await systemUnderTest.GetThemeNameAsync();

        // Assert
        result.Should().Be(appDataContent.ThemeName);
    }
}