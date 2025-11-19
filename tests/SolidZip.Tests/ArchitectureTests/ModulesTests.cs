using FluentAssertions;
using NetArchTest.Rules;
using SolidZip.Modules.Explorer;

namespace SolidZip.Tests.ArchitectureTests;

public class ModulesTests
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
}