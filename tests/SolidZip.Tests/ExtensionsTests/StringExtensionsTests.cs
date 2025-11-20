using FluentAssertions;
using SolidZip.Core.Extensions;

namespace SolidZip.Tests.ExtensionsTests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("dir\\myarch.zip\\data", '\\', '.', "dir\\myarch.zip")]
    [InlineData("dir\\myarch.zip\\", '\\', '.', "dir\\myarch.zip")]
    [InlineData("dir\\myarch.zip\\data\\data", '\\', '.', "dir\\myarch.zip")]
    [InlineData("dir\\myarchzip\\data\\data", '\\', '.', "dir\\myarchzip\\data\\data")]
    [InlineData("dir\\myarch.zip\\data\\data.txt", '\\', '.', "dir\\myarch.zip")]
    public void CutFromEnd_MustReturnExpectedResult(string value, char till, char stop, string expected)
    {
        //Arrange
        
        //Act
        var result = value.CutFromEnd(till, stop);
        //Assert
        result.Should().Be(expected);
    }
}