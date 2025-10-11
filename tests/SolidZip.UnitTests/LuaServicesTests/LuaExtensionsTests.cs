namespace SolidZip.UnitTests.LuaServicesTests;

public class LuaExtensionsTests
{
     [Theory]
     [AutoTestData]
     public void GetLuaExtensions_UnexistingEvent_ReturnEmptyCollection(string eventName, ConcurrentDictionary<string, ImmutableArray<string>> events)
     {
          //Arrange
          var logger = A.Fake<ILogger<LuaExtensions>>();
          var systemUnderTest = new LuaExtensions(logger);
          systemUnderTest.Load(events);
          //Act
          var result = systemUnderTest.GetLuaExtensions(eventName);
          //Assert
          result.Should().BeEmpty();
     }

     [Theory]
     [AutoTestData]
     public void GetLuaExtensions_ExistingEvent_ReturnCollectionOfTheLuaScripts(
          string eventName,
          ConcurrentDictionary<string, ImmutableArray<string>> events, 
          ImmutableArray<string> extensions)
     {
          //Arrange
          var logger = A.Fake<ILogger<LuaExtensions>>();
          var systemUnderTest = new LuaExtensions(logger);
          events.TryAdd(eventName, extensions);
          systemUnderTest.Load(events);
          //Act
          var result = systemUnderTest.GetLuaExtensions(eventName);
          //Assert
          result.Should().BeEquivalentTo(extensions);
     }
}