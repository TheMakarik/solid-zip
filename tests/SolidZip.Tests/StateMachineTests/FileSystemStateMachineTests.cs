using System.IO;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SolidZip.Core.Contracts.Archiving;
using SolidZip.Core.Enums;
using SolidZip.Factories;
using SolidZip.Modules.Archiving;
using SolidZip.Modules.StateMachines;
using TheMakarik.Testing.FileSystem;
using TheMakarik.Testing.FileSystem.Core;
using TheMakarik.Testing.FileSystem.Zip;

namespace SolidZip.Tests.StateMachineTests;

public class FileSystemStateMachineTests : IDisposable
{
    private readonly FileSystemStateMachine _systemUnderTests;
    private readonly IFileSystem _fileSystem;
    private string _directory;
    private string _zipArchive;
    private string _inZipFile;
    private string _inZipDirectoryFile;
    private string _inZipDirectory;

    public FileSystemStateMachineTests()
    {
        var supportedArchivesExtensions = new ArchiveSupportedExtensions([".zip"]);
        var provider = new ServiceCollection()
            .AddKeyedSingleton<IArchiveReader>(".zip", A.Dummy<IArchiveReader>())
            .BuildServiceProvider();
        var factory = new ArchiveReaderFactory(supportedArchivesExtensions,provider);

        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomInTempRootName()
            .AddDirectory("test-directory", out _directory)
            .AddZip("zip-archive", out _zipArchive, builder =>
                builder.AddFile("zip-archive-file.txt",  out _inZipFile, Guid.NewGuid().ToString())
                    .AddDirectory("inzip-directory", out _inZipDirectory, 
                        builder => builder
                            .AddFile("inzip-file.txt", out _inZipDirectoryFile, Guid.NewGuid().ToString())))
            .Build();
        
        
        _systemUnderTests =  new FileSystemStateMachine(factory, A.Dummy<ILogger<FileSystemStateMachine>>());
    }

    [Fact]
    public void AttemptToSwitchState_Archive_SetStateAsArchive()
    {
       //Arrange
       
       //Act
       _systemUnderTests.AttemptToSwitchState(_zipArchive,  out _);
       var result = _systemUnderTests.GetState();
       //Assert
       result.Should().Be(FileSystemState.Archive);
    }
    
    [Fact]
    public void AttemptToSwitchState_ArchiveAndDirectory_SetStateAsDirectory()
    {
        //Arrange
        _systemUnderTests.AttemptToSwitchState(_zipArchive,  out _);
        //Act
        _systemUnderTests.AttemptToSwitchState(_directory,  out _);
        var result = _systemUnderTests.GetState();
        //Assert
        result.Should().Be(FileSystemState.Directory);
    }
    
    [Fact]
    public void AttemptToSwitchState_InArchiveDirectory_SetStateAsArchive()
    {
        //Arrange
        
        //Act
        _systemUnderTests.AttemptToSwitchState(Path.Combine(_zipArchive, _inZipDirectory),  out _);
        var result = _systemUnderTests.GetState();
        //Assert
        result.Should().Be(FileSystemState.Archive);
    }
    
    [Fact]
    public void AttemptToSwitchState_InArchiveFile_SetStateAsArchive()
    {
        //Arrange
        
        //Act
        _systemUnderTests.AttemptToSwitchState(Path.Combine(_zipArchive, _inZipFile),  out _);
        var result = _systemUnderTests.GetState();
        //Assert
        result.Should().Be(FileSystemState.Archive);
    }
    
    [Fact]
    public void AttemptToSwitchState_InArchiveFileInDirectory_SetStateAsArchive()
    {
        //Arrange
        
        //Act
        _systemUnderTests.AttemptToSwitchState(Path.Combine(_zipArchive, _inZipDirectoryFile), out _);
        var result = _systemUnderTests.GetState();
        //Assert
        result.Should().Be(FileSystemState.Archive);
    }
    
    public void Dispose()
    {
        _fileSystem.Dispose();
    }
}