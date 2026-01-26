using FluentAssertions;
using NetArchTest.Rules;
using SolidZip.Core.Utils;
using SolidZip.Modules.Explorer;

namespace SolidZip.Tests.ArchitectureTests;

public class ArchTests
{
    [Fact]
    public void Modules_MustNoHaveDependenciesFromOtherModules()
    {
        // Arrange
        var assembly = typeof(ExplorerHistory).Assembly; //Just a mock type for loading assembly
        var types = Types.InAssembly(assembly);

        // Act
        var result = types
            .That()
            .ResideInNamespace("SolidZip.Modules")
            .ShouldNot()
            .HaveDependencyOnAny(
                "SolidZip\\.Modules\\.\\w+\\.",
                "SolidZip\\.Modules\\.\\w+\\.\\w+")
            .GetResult();

        // Assert
        result.IsSuccessful
            .Should()
            .BeTrue(result.ToString());
    }

    [Fact]
    public void Core_MustNoHaveDependenciesModules()
    {
        // Arrange
        var assembly = typeof(ConsoleAttacher).Assembly; //Just a mock type for loading assembly
        var types = Types.InAssembly(assembly);

        // Act
        var result = types
            .That()
            .ResideInNamespace("SolidZip.Core")
            .ShouldNot()
            .HaveDependencyOnAny(
                "SolidZip\\.Modules\\.\\w+\\.",
                "SolidZip\\.Modules\\.\\w+\\.\\w+")
            .GetResult();

        // Assert
        result.IsSuccessful
            .Should()
            .BeTrue(result.ToString());
    }
}