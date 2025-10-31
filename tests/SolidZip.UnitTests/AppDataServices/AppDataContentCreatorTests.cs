namespace SolidZip.UnitTests.AppDataServices;

public class AppDataContentCreatorTests
{
    private readonly ILogger<AppDataContentCreator> _loggerStub = A.Dummy<ILogger<AppDataContentCreator>>();
    private readonly IFileProxy _fileProxyMock = A.Fake<IFileProxy>();
    private readonly IFileStreamFactory _streamFactoryMock = A.Fake<IFileStreamFactory>();
    private readonly IJsonSerializer _serializerMock = A.Fake<IJsonSerializer>();

    [Theory]
    [AutoTestData]
    internal async Task CreateAsync_FileExists_DoNotCreateAnyFiles(
        IOptions<AppDataOptions> appDataOptionsMock,
        string userFilePath,
        string archiveFilePath)
    {
        // Arrange
        A.CallTo(() => _fileProxyMock.Exists(userFilePath)).Returns(true);
        A.CallTo(() => _fileProxyMock.Exists(archiveFilePath)).Returns(true);

        appDataOptionsMock.FakeAppDataOptions(userFilePath);

        var systemUnderTest = new AppDataContentCreator(_loggerStub, _fileProxyMock, appDataOptionsMock,
            _streamFactoryMock, _serializerMock);

        // Act
        await systemUnderTest.CreateAsync();

        // Assert
        A.CallTo(() =>
                _serializerMock.SerializeAsync(A<object>._, A<FileStream>._, A<string>._, A<JsonSerializerOptions>._))
            .MustNotHaveHappened();
    }


    [Theory]
    [AutoTestData]
    internal async Task CreateAsync_UserFileNotExists_CreateUserFile(
        IOptions<AppDataOptions> appDataOptionsMock,
        string userFilePath,
        string archiveFilePath)
    {
        // Arrange
        A.CallTo(() => _fileProxyMock.Exists(userFilePath)).Returns(false);
        A.CallTo(() => _fileProxyMock.Exists(archiveFilePath)).Returns(true);

        appDataOptionsMock.FakeAppDataOptions(userFilePath);

        var systemUnderTest = new AppDataContentCreator(_loggerStub, _fileProxyMock, appDataOptionsMock,
            _streamFactoryMock, _serializerMock);

        // Act
        await systemUnderTest.CreateAsync();

        // Assert
        A.CallTo(() => _streamFactoryMock.GetFactory(userFilePath, FileMode.Create))
            .MustHaveHappenedOnceExactly();
    }
    
}
