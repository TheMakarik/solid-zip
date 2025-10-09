
using SolidZip.Services.ProxiesServices.Abstractions;

namespace SolidZip.UnitTests.ExplorerServices;

public class ExplorerTests
{
    private readonly ILogger<Explorer> _loggerStub = A.Dummy<ILogger<Explorer>>();
    private readonly IDirectoryProxy _directoryProxy = A.Fake<IDirectoryProxy>();
    private readonly IOptions<ExplorerOptions> _options = A.Fake<IOptions<ExplorerOptions>>();
    
    [Theory]
    [AutoData]
    internal void GetDirectoryContent_WithRootDirectory_ReturnsLogicalDrivesAndSuccessResult(
        string[] additionalRootContent,
        string[] logicalDrives,
        string rootDirectory)
    {
        
        //Arrange
        A.CallTo(() => _options.Value)
            .Returns(new ExplorerOptions(){
             DeeperDirectoryName = string.Empty,
             RootDirectory = rootDirectory,
             RootDirectoryAdditionalContent = additionalRootContent
        });

        A.CallTo(() => _directoryProxy.GetLogicalDrives())
            .Returns(logicalDrives);

        A.CallTo(() => _directoryProxy.Exists(
                A<string>
                    .That
                    .Matches(@string => logicalDrives.Contains(@string) || @string == rootDirectory)))
            .Returns(true);
        
        
        var systemUnderTests = new Explorer(_loggerStub, _options, _directoryProxy);
        
        //Act
        var result = systemUnderTests.GetDirectoryContent(new FileEntity(rootDirectory, IsDirectory: true));
       
        //Assert
        result.Entities
            .Should()
            .Contain(logicalDrives.ToFileEntityCollection(isDirectory: true))
            .And
            .Contain(additionalRootContent.ToFileEntityCollection(isDirectory: true));
        
        result.Result
            .Should()
            .Be(ExplorerResult.Success);
    }
    
    [Theory]
    [AutoData]
    internal void GetDirectoryContent_WithRootDirectoryAndLinuxDistro_SkipsLinuxDistroDrives(
        string[] additionalRootContent,
        string[] logicalDrives,
        string[] linuxDrives,
        string rootDirectory)
    {
        
        //Arrange
        A.CallTo(() => _options.Value)
            .Returns(new ExplorerOptions(){
                DeeperDirectoryName = string.Empty,
                RootDirectory = rootDirectory,
                RootDirectoryAdditionalContent = additionalRootContent
            });

        A.CallTo(() => _directoryProxy.GetLogicalDrives())
            .Returns(logicalDrives
                .Concat(linuxDrives)
                .ToArray());

        A.CallTo(() => _directoryProxy.Exists(
                A<string>
                    .That
                    .Matches(@string => logicalDrives.Contains(@string))))
            .Returns(true);
        
        
        var systemUnderTests = new Explorer(_loggerStub, _options, _directoryProxy);
        
        //Act
        var result = systemUnderTests.GetDirectoryContent(new FileEntity(rootDirectory, IsDirectory: true));
       
        //Assert
        result.Entities
            .Should()
            .Contain(logicalDrives
                .ToFileEntityCollection(isDirectory: true))
            .And
            .Contain(additionalRootContent
                .ToFileEntityCollection(isDirectory: true))
            .And.NotContain(linuxDrives
                .ToFileEntityCollection(isDirectory: true));
    }
    
    [Theory]
    [AutoData]
    internal void GetDirectoryContent_WithFile_ReturnNotDirectoryResultAndEmptyResult(string fileName)
    {
        //Arrange
        var systemUnderTests = new Explorer(_loggerStub, _options, _directoryProxy);
        
        //Act
        var result = systemUnderTests.GetDirectoryContent(new FileEntity(fileName, IsDirectory: false));

        //Assert
        result.Entities
            .Should()
            .BeEmpty();
        result.Result
            .Should()
            .Be(ExplorerResult.NotDirectory);
    }
    
    [Theory]
    [AutoData]
    internal void GetDirectoryContent_WithDirectory_ReturnSuccessResultAndDirectoryContent(
        string directory,
        string[] directoryDirectoriesContent,
        string[] directoryFilesContent)
    {
        //Arrange
        A.CallTo(() => _directoryProxy.EnumerateDirectories(directory))
            .Returns(directoryDirectoriesContent);
        A.CallTo(() => _directoryProxy.EnumerateFiles(directory))
            .Returns(directoryFilesContent);
        
        var systemUnderTests = new Explorer(_loggerStub, _options, _directoryProxy);
        
        //Act
        var result = systemUnderTests.GetDirectoryContent(new FileEntity(directory, IsDirectory: true));

        //Assert
        result.Entities
            .Should()
            .Contain(directoryDirectoriesContent.ToFileEntityCollection(isDirectory: true))
            .And
            .Contain(directoryFilesContent.ToFileEntityCollection(isDirectory: false));
        result.Result
            .Should()
            .Be(ExplorerResult.Success);
    }
    
    [Theory]
    [AutoData]
    internal void GetDirectoryContent_WithUnexistedDirectory_ReturnUnexistingDirectoryResultAndEmptyContent(
        string directory,
        string[] directoryDirectoriesContent)
    {
        //Arrange
        A.CallTo(() => _directoryProxy.EnumerateDirectories(directory))
            .Returns(directoryDirectoriesContent);

        A.CallTo(() => _directoryProxy.Exists(directory))
            .Returns(false);
        
        var systemUnderTests = new Explorer(_loggerStub, _options, _directoryProxy);
        
        //Act
        var result = systemUnderTests.GetDirectoryContent(new FileEntity(directory, IsDirectory: true));

        //Assert
        result.Entities
            .Should()
            .BeEmpty();
        result.Result
            .Should()
            .Be(ExplorerResult.UnexistingDirectory);
    }
    
    [Theory]
    [AutoData]
    internal void GetDirectoryContent_WithNoAccess_ReturnUnauthorizedAccess(string directory)
    {
        //Arrange
        A.CallTo(() => _directoryProxy.EnumerateDirectories(directory))
            .Throws<UnauthorizedAccessException>();

        A.CallTo(() => _directoryProxy.EnumerateFiles(directory))
            .Throws<UnauthorizedAccessException>();
        
        A.CallTo(() => _directoryProxy.Exists(directory))
            .Returns(true);
        
        var systemUnderTests = new Explorer(_loggerStub, _options, _directoryProxy);
        
        //Act
        var result = systemUnderTests.GetDirectoryContent(new FileEntity(directory, IsDirectory: true));

        //Assert
        result.Result
            .Should()
            .Be(ExplorerResult.UnauthorizedAccess);
        
    }
}