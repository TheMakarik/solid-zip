using TheMakarik.Testing.FileSystem;

namespace SolidZip.Testing.ArchiveTests;

public class ZipArchiveCreatorTests
{
    private readonly object _fileSystem;

    public ZipArchiveCreatorTests()
    {
        _fileSystem = FileSystem.BeginBuilding()
            .Build();
    }
}