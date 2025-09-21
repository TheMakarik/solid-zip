
namespace SolidZip.UnitTests.AppDataServices;

public class AppDataContentCreatorTests
{
    
    [Theory]
    [AutoTestData]
    internal async Task CreateIfNotExistsAsync_FileExists_DoNotCreateConfigurationFile(
        [Frozen] ILogger<AppDataContentCreator> logger,
        IFileProxy fileProxy,
        IOptions<AppDataOptions> options,
        [Frozen] IFileStreamFactory streamFactory,
        [Frozen] IJsonSerializer serializer,
        [Frozen] AppDataContent appDataContent,
        string file
        )
    {
      //Arrange
      A.CallTo(() => fileProxy.Exists(file))
          .Returns(true);
      
      A.CallTo(() => options.Value).Returns(new AppDataOptions
      {
          DataJsonFilePath = file,
          Defaults = appDataContent
      });
      
      var appDataContentCreator = new AppDataContentCreator(logger, fileProxy, options, streamFactory, serializer);
      
      //Act
      await appDataContentCreator.CreateAsync();
      
      //Assert
      A.CallTo(serializer)
          .Where(fakeCall => fakeCall.Method.Name == nameof(serializer.SerializeAsync))
          .MustNotHaveHappened();
    }
}