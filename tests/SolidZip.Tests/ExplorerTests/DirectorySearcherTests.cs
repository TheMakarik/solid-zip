using System.IO;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolidZip.Core.Options;
using SolidZip.Modules.Explorer;

namespace SolidZip.Tests.ExplorerTests;

public class DirectorySearcherTests : IDisposable
{
    private const int TestDirectoryContentCount = 30;
    private readonly IOptions<ExplorerOptions> _options;
    private readonly DirectorySearcher _systemUnderTests;
    private readonly string TestDirectory = "TEST" + Random.Shared.NextDouble().ToString("00");

    public DirectorySearcherTests()
    {
        Directory.CreateDirectory(TestDirectory);
        for (var i = 1; i <= TestDirectoryContentCount; i++)
        {
            var path = Path.Combine(TestDirectory, i.ToString());
            Directory.CreateDirectory(path);
        }

        _options = A.Fake<IOptions<ExplorerOptions>>();
        A.CallTo(() => _options.Value).Returns(new ExplorerOptions
        {
            DeeperDirectory = string.Empty,
            RootDirectory = "sz\\"
        });

        _systemUnderTests = new DirectorySearcher(A.Dummy<ILogger<DirectorySearcher>>(), _options);
    }

    public void Dispose()
    {
        Directory.Delete(TestDirectory, true);
    }

    [Fact]
    public void Search_RootDirectory_ReturnsRootContentWithRootPrefix()
    {
        //Arrange
        var expected = _options.Value.RootDirectory + Directory.GetLogicalDrives().First();

        //Act
        var result = _systemUnderTests.Search(_options.Value.RootDirectory, string.Empty);

        //Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Search_PathWithRootPrefix_ReturnsContentWithRootPrefix()
    {
        //Arrange
        var searchTarget = Path.Combine(_options.Value.RootDirectory, TestDirectory);
        var expected = _options.Value.RootDirectory + Directory.GetDirectories(TestDirectory).First();

        //Act
        var result = _systemUnderTests.Search(searchTarget, string.Empty);

        //Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Search_PathWithoutRootPrefix_ReturnsContentWithoutRootPrefix()
    {
        //Arrange
        var expected = Directory.GetDirectories(TestDirectory).First();

        //Act
        var result = _systemUnderTests.Search(TestDirectory, string.Empty);

        //Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Search_AllDirectoriesWasSearched_DropStates()
    {
        //Arrange
        var expected = Directory.GetDirectories(TestDirectory).First();
        for (var i = 1; i <= TestDirectoryContentCount; i++)
            _systemUnderTests.Search(TestDirectory, string.Empty);
        //Act
        var result = _systemUnderTests.Search(TestDirectory, string.Empty);
        //Assert
        result.Should().Be(expected);
    }
}