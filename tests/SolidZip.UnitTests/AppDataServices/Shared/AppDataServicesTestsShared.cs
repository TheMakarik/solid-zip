namespace SolidZip.UnitTests.AppDataServices.Shared;

public static class AppDataServicesTestsShared
{
    public static void FakeAppDataOptions(this IOptions<AppDataOptions> options, string filePath)
    {
        A.CallTo(() => options.Value)
            .Returns(new()
            {
                Defaults = default,
                DataJsonFilePath = filePath
            });
    }
    
    public static void FakeArchiveOptions(this IOptions<ArchiveOptions> options, string archiveFilePath)
    {
        A.CallTo(() => options.Value)
            .Returns(new ArchiveOptions
            {
                ArchiveConfigurationPath = archiveFilePath,
                DefaultConfiguration = new ArchiveConfiguration()
            });
    }

    public static IEnumerable<FileEntity> ToFileEntityCollection(this string[] paths, bool isDirectory)
    {
        return paths.Select(p => new FileEntity(p, isDirectory));
    }
}