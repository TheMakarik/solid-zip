namespace SolidZip.UnitTests.ExplorerServices;

public class ExplorerHistoryTests
{
    private ExplorerHistory _systemUnderTests = new( A.Dummy<ILogger<ExplorerHistory>>());
    
    [Fact]
    public void CanRedo_OnInstanceCreating_False()
    {
        //Arrange
        
        //Act
        
        //Assert
        _systemUnderTests.CanRedo
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void CanUndo_OnInstanceCreating_False()
    {
        //Arrange
        
        //Act
        
        //Assert
        _systemUnderTests.CanUndo
            .Should()
            .BeFalse();
    }
    
    [Theory]
    [AutoData]
    public void CanUndo_AfterAddingElement_False(FileEntity directory)
    {
        //Arrange
        AddEntities(directory);
        //Act
        
        //Assert
        _systemUnderTests.CanUndo
            .Should()
            .BeFalse();
    }
    
    [Theory]
    [AutoData]
    public void CanUndo_AfterMoreThanOnceEntities_True(FileEntity[] directories)
    {
        //Arrange
        AddEntities(directories);
        //Act
       
        //Assert
        _systemUnderTests.CanUndo
            .Should()
            .BeTrue();
    }
    
    [Theory]
    [AutoData]
    public void CanRedo_AfterMoreThanOnceEntitiesAndUndo_True(FileEntity[] directories)
    {
        //Arrange
        AddEntities(directories);
        _systemUnderTests.Undo();
        //Act
       
        //Assert
        _systemUnderTests.CanRedo
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Undo_CanUndoIsFalse_ThrowsException()
    {
        //Arrange
        
        //Act and Assert
        Assert.Throws<InvalidOperationException>(() => _systemUnderTests.Undo());
    }
    
    [Fact]
    public void Redo_CanRedoIsFalse_ThrowsException()
    {
        //Arrange
        
        //Act and Assert
        Assert.Throws<InvalidOperationException>(() => _systemUnderTests.Redo());
    }
    
    
    [Theory]
    [AutoData]
    public void Undo_CanUndoIsTrue_MustSetPreviousValueToCurrentDirectory(FileEntity[] directories)
    {
        //Arrange
        AddEntities(directories);
        
        //Act
        _systemUnderTests.Undo();
        
        //Assert
        _systemUnderTests.CurrentEntity
            .Should()
            .Be(directories
                .Skip(directories.Length - 2)
                .First());
    }
    
    [Theory]
    [AutoData]
    public void Redo_CanRedoIsTrue_MustSetNextValueToCurrentDirectory(FileEntity[] directories)
    {
        //Arrange
        AddEntities(directories);
        
        //Act
        _systemUnderTests.Undo();
        _systemUnderTests.Redo();
        
        //Assert
        _systemUnderTests.CurrentEntity
            .Should()
            .Be(directories.Last());
    }
    
    private void AddEntities(params FileEntity[] entites)
    {
        foreach (var entity in entites)
            _systemUnderTests.CurrentEntity = entity;
    }
}