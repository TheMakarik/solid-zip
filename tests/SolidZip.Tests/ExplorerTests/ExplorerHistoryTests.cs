using System.ComponentModel;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SolidZip.Core.Models;
using SolidZip.Modules.Explorer;

namespace SolidZip.Tests.ExplorerTests;

[Category(Categories.MainTests)]
public class ExplorerHistoryTests
{
    private readonly ExplorerHistory _history = new(A.Dummy<ILogger<ExplorerHistory>>());

    [Fact]
    [Category(Categories.StateProperties)]
    public void CanRedo_OnStart_False()
    {
        //Assert
        _history.CanRedo
            .Should()
            .BeFalse();
    }

    [Fact]
    [Category(Categories.StateProperties)]
    public void CanUndo_OnStart_False()
    {
        //Assert
        _history.CanUndo
            .Should()
            .BeFalse();
    }

    [Theory]
    [AutoData]
    [Category(Categories.StateProperties)]
    public void CanUndo_AfterAddingOneElement_False(FileEntity directory)
    {
        //Arrange
        _history.CurrentEntity = directory;

        //Act
        var result = _history.CanUndo;

        //Assert
        result.Should()
            .BeFalse();
    }

    [Theory]
    [AutoData]
    [Category(Categories.StateProperties)]
    public void CanUndo_AfterAddingToElements_True(FileEntity firstDirectory, FileEntity secondDirectory)
    {
        //Arrange
        _history.CurrentEntity = firstDirectory;
        _history.CurrentEntity = secondDirectory;

        //Act
        var result = _history.CanUndo;

        //Assert
        result.Should()
            .BeTrue();
    }

    [Theory]
    [AutoData]
    [Category(Categories.StateProperties)]
    public void CanRedo_AfterAddingToElementsAndUndo_True(FileEntity firstDirectory, FileEntity secondDirectory)
    {
        //Arrange
        _history.CurrentEntity = firstDirectory;
        _history.CurrentEntity = secondDirectory;
        _history.Undo();

        //Act
        var result = _history.CanRedo;

        //Assert
        result.Should()
            .BeTrue();
    }

    [Theory]
    [AutoData]
    [Category(Categories.MainLogicMethods)]
    public void Undo_WhenAble_ChangeCurrentDirectoryToFirst(FileEntity firstDirectory, FileEntity secondDirectory)
    {
        //Arrange
        _history.CurrentEntity = firstDirectory;
        _history.CurrentEntity = secondDirectory;
        _history.Undo();

        //Act
        var result = _history.CurrentEntity;

        //Assert
        result.Should()
            .Be(firstDirectory);
    }

    [Theory]
    [AutoData]
    [Category(Categories.MainLogicMethods)]
    public void Undo_WhenUnable_ThrowsException(FileEntity directory)
    {
        //Arrange
        _history.CurrentEntity = directory;

        //Act
        var action = _history.Undo;

        //Assert
        action.Should()
            .Throw<InvalidOperationException>();
    }

    [Theory]
    [AutoData]
    [Category(Categories.MainLogicMethods)]
    public void Redo_WhenUnable_ThrowsException(FileEntity directory)
    {
        //Arrange
        _history.CurrentEntity = directory;

        //Act
        var action = _history.Redo;

        //Assert
        action.Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    [Category(Categories.MainLogicMethods)]
    public void CurrentEntity_WhenHistoryIsEmpty_ThrowsException()
    {
        // Act
        var action = () => _history.CurrentEntity;

        // Assert
        action.Should()
            .Throw<InvalidOperationException>();
    }

    [Theory]
    [AutoData]
    [Category(Categories.MainLogicMethods)]
    public void CurrentEntity_SetNewValue_TrimsFutureHistory(FileEntity first, FileEntity second, FileEntity third,
        FileEntity newEntity)
    {
        // Arrange
        _history.CurrentEntity = first;
        _history.CurrentEntity = second;
        _history.CurrentEntity = third;
        _history.Undo();

        // Act
        _history.CurrentEntity = newEntity;

        // Assert
        _history.CurrentEntity
            .Should()
            .Be(newEntity);
        _history.CanRedo
            .Should()
            .BeFalse();
        _history.CanUndo
            .Should()
            .BeTrue();
    }
}