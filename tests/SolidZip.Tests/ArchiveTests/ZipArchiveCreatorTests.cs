using TheMakarik.Testing.FileSystem;
using TheMakarik.Testing.FileSystem.Core;

namespace SolidZip.Tests.ArchiveTests;

public class ZipArchiveCreatorTests
{
    private readonly object _fileSystem;

    public ZipArchiveCreatorTests()
    {
        _fileSystem = FileSystem.BeginBuilding()
            .Build();
        
    }
}