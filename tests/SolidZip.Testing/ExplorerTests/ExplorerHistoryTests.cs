using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SolidZip.Core.Models;
using SolidZip.Modules.Explorer;

namespace SolidZip.Testing.ExplorerTests;

public class ExplorerHistoryTests
{
    private readonly ExplorerHistory _systemUnderTests = new(A.Dummy<ILogger<ExplorerHistory>>());

    [Fact]
    public void CanRedo_OnStart_False()
    {
        //Assert
        _systemUnderTests.CanRedo
            .Should()
            .BeFalse();
    }

    [Fact]
    public void CanUndo_OnStart_False()
    {
        //Assert
        _systemUnderTests.CanUndo
            .Should()
            .BeFalse();
    }

    [Theory]
    [AutoData]
    public void CanUndo_AfterAddingOneElement_False(FileEntity directory)
    {
        //Arrange
        _systemUnderTests.CurrentEntity = directory;

        //Act
        var result = _systemUnderTests.CanUndo;

        //Assert
        result.Should()
            .BeFalse();
    }

    [Theory]
    [AutoData]
    public void CanUndo_AfterAddingToElements_True(FileEntity firstDirectory, FileEntity secondDirectory)
    {
        //Arrange
        _systemUnderTests.CurrentEntity = firstDirectory;
        _systemUnderTests.CurrentEntity = secondDirectory;

        //Act
        var result = _systemUnderTests.CanUndo;

        //Assert
        result.Should()
            .BeTrue();
    }

    [Theory]
    [AutoData]
    public void CanRedo_AfterAddingToElementsAndUndo_True(FileEntity firstDirectory, FileEntity secondDirectory)
    {
        //Arrange
        _systemUnderTests.CurrentEntity = firstDirectory;
        _systemUnderTests.CurrentEntity = secondDirectory;
        _systemUnderTests.Undo();

        //Act
        var result = _systemUnderTests.CanRedo;

        //Assert
        result.Should()
            .BeTrue();
    }

    [Theory]
    [AutoData]
    public void Undo_WhenAble_ChangeCurrentDirectoryToFirst(FileEntity firstDirectory, FileEntity secondDirectory)
    {
        //Arrange
        _systemUnderTests.CurrentEntity = firstDirectory;
        _systemUnderTests.CurrentEntity = secondDirectory;
        _systemUnderTests.Undo();

        //Act
        var result = _systemUnderTests.CurrentEntity;

        //Assert
        result.Should()
            .Be(firstDirectory);
    }

    [Theory]
    [AutoData]
    public void Undo_WhenUnable_ThrowsException(FileEntity directory)
    {
        //Arrange
        _systemUnderTests.CurrentEntity = directory;

        //Act
        var action = _systemUnderTests.Undo;

        //Assert
        action.Should()
            .Throw<InvalidOperationException>();
    }

    [Theory]
    [AutoData]
    public void Redo_WhenUnable_ThrowsException(FileEntity directory)
    {
        //Arrange
        _systemUnderTests.CurrentEntity = directory;

        //Act
        var action = _systemUnderTests.Redo;

        //Assert
        action.Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public void CurrentEntity_WhenHistoryIsEmpty_ThrowsException()
    {
        // Act
        var action = () => _systemUnderTests.CurrentEntity;

        // Assert
        action.Should()
            .Throw<InvalidOperationException>();
    }

    [Theory]
    [AutoData]
    public void CurrentEntity_SetNewValue_TrimsFutureHistory(FileEntity first, FileEntity second, FileEntity third,
        FileEntity newEntity)
    {
        // Arrange
        _systemUnderTests.CurrentEntity = first;
        _systemUnderTests.CurrentEntity = second;
        _systemUnderTests.CurrentEntity = third;
        _systemUnderTests.Undo();

        // Act
        _systemUnderTests.CurrentEntity = newEntity;

        // Assert
        _systemUnderTests.CurrentEntity
            .Should()
            .Be(newEntity);
        _systemUnderTests.CanRedo
            .Should()
            .BeFalse();
        _systemUnderTests.CanUndo
            .Should()
            .BeTrue();
    }
}