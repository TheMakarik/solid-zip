namespace SolidZip.UnitTests.AppDataServices;

public class AppDataContentCreatorTests
{
    private readonly ILogger<AppDataContentCreator> _loggerStub = A.Dummy<ILogger<AppDataContentCreator>>();
    private readonly IFileProxy _fileProxyMock = A.Fake<IFileProxy>();
    private readonly IFileStreamFactory _streamFactoryMock = A.Fake<IFileStreamFactory>();
    private readonly IJsonSerializer _serializerMock = A.Fake<IJsonSerializer>();

    [Theory]
    [AutoTestData]
    internal async Task CreateIfNotExistsAsync_FileExists_DoNotCreateConfigurationFile(
        IOptions<AppDataOptions> optionsMock,
        string file)
    {
        //Arrange
        A.CallTo(() => _fileProxyMock.Exists(file))
            .Returns(true);
      
        optionsMock.FakeFromShared(file);
      
        var systemUnderTest = new AppDataContentCreator(_loggerStub, _fileProxyMock, optionsMock, _streamFactoryMock, _serializerMock);
      
        //Act
        await systemUnderTest.CreateAsync();
      
        //Assert
        A.CallTo(_serializerMock)
            .Where(fakeCall => fakeCall.Method.Name == nameof(_serializerMock.SerializeAsync))
            .MustNotHaveHappened();
    }
    
    [Theory]
    [AutoTestData]
    internal async Task CreateIfNotExistsAsync_FileNotExists_CreateFileWithPathFromOptions(
        IOptions<AppDataOptions> optionsMock,
        string file)
    {
        //Arrange
        optionsMock.FakeFromShared(file);
      
        var systemUnderTest = new AppDataContentCreator(_loggerStub, _fileProxyMock, optionsMock, _streamFactoryMock, _serializerMock);
      
        //Act
        await systemUnderTest.CreateAsync();
      
        //Assert
        A.CallTo(() => _streamFactoryMock.GetFactory(file, FileMode.Create))
            .MustHaveHappenedOnceExactly();
    }
}