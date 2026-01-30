using FluentAssertions;
using SolidZip.Core.Extensions;

namespace SolidZip.Testing.ExtensionsTests;

public class RangeWhereTests
{
    [Theory]
    [InlineData(100, 200, 150)]
    public void RangeWhere_MustFoundValues(int start, int end, int search)
    {
        //Arrange
        var array = Enumerable.Range(1, end).ToArray();
        //Act
        var result = array.RangeWhere(x => x <= search && x >= start);
        //Assert
        result.Should().BeEquivalentTo(array.Where(x => x <= search && x >= start));
    }
}