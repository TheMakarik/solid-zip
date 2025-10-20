using System.Collections;

namespace SolidZip.Services.ExplorerServices;

internal sealed class ExplorerHistory(ILogger<ExplorerHistory> logger) : IExplorerHistory
{
    private const string AddedNewEntityLogMessage = "Added new entity to history: {Path}";
    private const string PerformingUndoLogMessage = "Performing undo from {Current} to {Previous}";
    private const string PerformingRedoLogMessage = "Performing redo from {Current} to {Next}";
    private const string CannotUndoLogMessage = "Cannot undo, history is empty or at the beginning";
    private const string CannotRedoLogMessage = "Cannot redo, no future states available";
    
    private const string HistoryTrimmedLogMessage = "History trimmed, removed {Count} future states";
    private const string HistoryIsEmptyExceptionMessage = "Cannot get current entity from empty history";
    private const string CannotUndoExceptionMessage = "Cannot undo operation. History is empty or current entity is the first one in history";
    private const string CannotRedoExceptionMessage = "Cannot redo operation. No future states available after current entity";

    private readonly LinkedList<FileEntity> _history = new();
    private LinkedListNode<FileEntity> _currentNode;

    public bool CanRedo => _currentNode?.Next is not  null;
    public bool CanUndo => _currentNode?.Previous is not null;

    public FileEntity CurrentEntity
    {
        get
        {
            ValidateHistoryIsNotEmpty();
            return _currentNode.Value;
        }
        set
        {
            TrimFutureStates();
            AddNewEntity(value);
            logger.LogDebug(AddedNewEntityLogMessage, value.Path);
        }
    }

    public void Undo()
    {
        ValidateCanUndo();
        
        var previousEntity = _currentNode.Previous.Value;
        logger.LogDebug(PerformingUndoLogMessage, _currentNode.Value.Path, previousEntity.Path);
        _currentNode = _currentNode.Previous;
    }

    public void Redo()
    {
        ValidateCanRedo();

        var nextEntity = _currentNode.Next.Value;
        logger.LogDebug(PerformingRedoLogMessage, _currentNode.Value.Path, nextEntity.Path);
        _currentNode = _currentNode.Next;
    }

    public IEnumerator<FileEntity> GetEnumerator()
    {
        var currentNode = _history.First;
        while (currentNode != null)
        {
            yield return currentNode.Value;
            currentNode = currentNode.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void TrimFutureStates()
    {
        if (_currentNode?.Next is null) 
            return;

        var trimCount = 0;
        var nodeToRemove = _currentNode.Next;
        
        while (nodeToRemove is not null)
        {
            var nextNode = nodeToRemove.Next;
            _history.Remove(nodeToRemove);
            nodeToRemove = nextNode;
            trimCount++;
        }

        logger.LogDebug(HistoryTrimmedLogMessage, trimCount);
    }

    private void AddNewEntity(FileEntity entity)
    {
        var newNode = new LinkedListNode<FileEntity>(entity);
        
        if (_history.Count == 0)
            _history.AddFirst(newNode);
        else
            _history.AddAfter(_currentNode, newNode);
        _currentNode = newNode;
    }

    private void ValidateHistoryIsNotEmpty()
    {
        if (_currentNode is null)
            throw new InvalidOperationException(HistoryIsEmptyExceptionMessage);
    }

    
    private void ValidateCanUndo()
    {
        if (CanUndo)
            return;
        
        logger.LogError(CannotUndoLogMessage);
        throw new InvalidOperationException(CannotUndoExceptionMessage);
        
    }

    private void ValidateCanRedo()
    {
        if (CanRedo)
            return;
        
        logger.LogError(CannotRedoLogMessage);
        throw new InvalidOperationException(CannotRedoExceptionMessage);
        
    }
}