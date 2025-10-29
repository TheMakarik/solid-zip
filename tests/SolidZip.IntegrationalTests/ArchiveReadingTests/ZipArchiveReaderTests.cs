namespace SolidZip.IntegrationalTests.ArchiveReadingTests;

public class ZipArchiveReaderTests : IDisposable
{
   private static readonly string Temp = Environment.GetEnvironmentVariable("TEMP") ?? string.Empty;
   private readonly ILogger<ZipArchiveReader> _logger = A.Dummy<ILogger<ZipArchiveReader>>();
   private readonly ArchiveConfiguration _archiveConfiguration = A.Fake<ArchiveConfiguration>();
   private ImmutableArray<string> _archiveFiles;
   private string _archivePath = Path.Combine(Temp, Guid.NewGuid() + ".zip");
      

   [Theory]
   [InlineData(0)]
   [InlineData(4)]
   public void GetEntries_Root_ReturnsRootContent(int count)
   {
      //Arrange
      CreateArchive(count, string.Empty);

      using var systemUnderTests = new ZipArchiveReader(_logger, _archiveConfiguration);
      systemUnderTests.SetPath(_archivePath);
      
      //Act
      var result = systemUnderTests.GetEntries(new FileEntity(string.Empty,  IsDirectory: true, IsArchiveEntry: true )); //Emulate root directory
     
      //Assert
      result
         .Select(e => e.Path)
         .ToArray()
         .Should()
         .BeEquivalentTo(_archiveFiles.Select(p => Path.GetFileName(p) ?? string.Empty));
   }
   
   [Theory]
   [InlineData(0, "dir")]
   [InlineData(4,"dir")]
   public void GetEntries_Directory_ReturnsDirectoryContent(int count, string directory)
   {
      //Arrange
      CreateArchive(count, directory);

      using var systemUnderTests = new ZipArchiveReader(_logger, _archiveConfiguration);
      systemUnderTests.SetPath(_archivePath);
      
      //Act
      var result = systemUnderTests.GetEntries(new FileEntity(directory,  IsDirectory: true, IsArchiveEntry: true )); //Emulate root directory
     
      //Assert
      result
         .Select(e => Path.GetFileName(e.Path))
         .ToArray()
         .Should()
         .BeEquivalentTo(_archiveFiles.Select(p => Path.GetFileName(p)));
   }
   
   [Theory]
   [InlineData(0)]
   [InlineData(4)]
   public void GetEntries_ReturnsEntityAsArchiveEntry(int count)
   {
      //Arrange
      CreateArchive(count, string.Empty);

      using var systemUnderTests = new ZipArchiveReader(_logger, _archiveConfiguration);
      systemUnderTests.SetPath(_archivePath);
      
      //Act
      var result = systemUnderTests.GetEntries(new FileEntity(string.Empty,  IsDirectory: true, IsArchiveEntry: true )); //Emulate root directory
     
      //Assert
      result
         .Select(e => e.IsArchiveEntry)
         .Should()
         .AllBeEquivalentTo(true);
   }
   
   [Theory]
   [InlineData(0, 2, "dir")]
   [InlineData(4, 5, "dir")]
   public void GetEntries_ReturnsOnlyRequiredContent(int rootCount, int directoryCount, string directory)
   {
      //Arrange
      CreateArchive(rootCount, string.Empty);
      using(var zip = ZipFile.Read(_archivePath))
         FillArchive(directoryCount, directory, zip);
      
      using var systemUnderTests = new ZipArchiveReader(_logger, _archiveConfiguration);
      systemUnderTests.SetPath(_archivePath);
      
      //Act
      var rootResult = systemUnderTests.GetEntries(new FileEntity(string.Empty,  IsDirectory: true, IsArchiveEntry: true )); //Emulate root directory
      var directoryResult = systemUnderTests.GetEntries(new FileEntity(directory, IsDirectory: true, IsArchiveEntry: true));
      
      //Assert
      rootResult
         .Should()
         .HaveCount(rootCount);

      directoryResult
         .Should()
         .HaveCount(directoryCount);
   }

   private void CreateArchive(int count, string directory)
   {
      using var zip = new ZipFile();
      FillArchive(count, directory, zip);
   }

   private void FillArchive(int count, string directory, ZipFile zip)
   {
      _archiveFiles = CreateArchiveFiles(count);
      foreach (var file in _archiveFiles)
         zip.AddFile(file, directory);
      zip.Save(_archivePath);
   }
   
   private ImmutableArray<string> CreateArchiveFiles(int count)
   {
      var files = Enumerable.Range(0, count)
         .Select(integer => Path.Combine(Temp, integer.ToString()))
         .ToImmutableArray();
      foreach (var file in files)
         File.Create(file).Dispose();
      return files;
   }

   public void Dispose()
   {
      File.Delete(_archivePath);
      foreach (var path in _archiveFiles)
         File.Delete(path);
   }
}