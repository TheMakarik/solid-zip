using FluentAssertions;
using NetArchTest.Rules;

namespace SolidZip.Tests.ArchitectureTests;

public class ModulesTests
{
    [Fact]
    public void Modules_MustNoHaveDependenciesFromOtherModules()
    {
        //Arrange
        var types = Types.InNamespace("SolidZip");
        
        //Act
        var result = types
            .That()
            .ResideInNamespace("SolidZip.Modules")
            .ShouldNot()
            .HaveDependencyOnAny("SolidZip.Modules")
            .GetResult();
        
        //Assert
        result.IsSuccessful
            .Should()
            .BeTrue();
    }
}