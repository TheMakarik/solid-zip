using TheMakarik.Testing.FileSystem;

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