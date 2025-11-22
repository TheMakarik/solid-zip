using System.IO;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Ionic.Zip;
using Microsoft.Extensions.Logging;
using SolidZip.Core.Models;
using SolidZip.Modules.Archiving;

namespace SolidZip.Tests.ArchiveTests;

public class ZipArchiveReaderTests : IDisposable
{
    private static readonly string[] ArchiveFiles = ["file-1.txt", "file-2.txt", "file-3.txt", "file-4.txt"];
    private const string ArchiveDirectory = "dir/";
    private const string ArchiveDeeperDirectory = "dir/dir-deeper/";

    private readonly string _testDirectory = Guid.NewGuid().ToString();
    private readonly string _archivePath;
    private readonly ILogger<ZipArchiveReader> _loggerStub = A.Dummy<ILogger<ZipArchiveReader>>();
    
    public ZipArchiveReaderTests()
    {
        Directory.CreateDirectory(_testDirectory);
    
        _archivePath = Path.Combine(_testDirectory, "ZIP.zip");
        using var zip = new ZipFile();
        foreach (var fileName in ArchiveFiles)
            zip.AddEntry(fileName, $"Content of {fileName}");
    
      
        zip.AddDirectoryByName(ArchiveDirectory);
        
        foreach (var fileName in ArchiveFiles)
        {
            var entryName = ArchiveDirectory + fileName;
            zip.AddEntry(entryName, $"Content of {fileName} in directory");
        }
        
        zip.AddDirectoryByName(ArchiveDeeperDirectory);
    
      
        foreach (var fileName in ArchiveFiles)
        {
            var entryName = ArchiveDeeperDirectory + fileName;
            zip.AddEntry(entryName, $"Content of {fileName} in deeper directory");
        }
    
        zip.Save(_archivePath);
    }

    [Theory]
    [AutoData]
    public void GetEntries_OnNotArchiveEntry_ThrowsException(FileEntity directory)
    {
        //Arrange
        using var systemUnderTests = new ZipArchiveReader(_loggerStub);
        //Act
        var action = systemUnderTests.GetEntries;
        //Assert
        Assert.Throws<InvalidOperationException>(
            () => action(directory with { IsArchiveEntry = false }));
    }

    [Theory]
    [AutoData]
    public void GetEntries_Root_ReturnsRootFilesAndDirectories(FileEntity entry)
    {
        //Arrange
        var rootEntry = entry with { IsArchiveEntry = true, Path = string.Empty, IsDirectory = true };
        using var systemUnderTests = new ZipArchiveReader(_loggerStub);
        systemUnderTests.SetPath(_archivePath);
        //Act
        var result = systemUnderTests.GetEntries(rootEntry).Value?.ToArray();
        //Assert
        result?
            .Length
            .Should()
            .Be(ArchiveFiles.Length + 1); //One directory also included
    }
    
    [Theory]
    [AutoData]
    public void GetEntries_Directory_ReturnsDirectoryFilesAndDirectories(FileEntity entry)
    {
        //Arrange
        var directoryEntry = entry with { IsArchiveEntry = true, Path = ArchiveDirectory, IsDirectory = true };
        using var systemUnderTests = new ZipArchiveReader(_loggerStub);
        systemUnderTests.SetPath(_archivePath);
        //Act
        var result = systemUnderTests.GetEntries(directoryEntry).Value?.ToArray();
        //Assert
        result?
            .Length
            .Should()
            .Be(ArchiveFiles.Length + 1); //One directory also included
    }
    
    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, recursive: true);
    }
}