namespace SolidZip.Modules.Explorer;

public sealed class ExplorerHistory(ILogger<ExplorerHistory> logger) : IExplorerHistory
{
    private readonly LinkedList<FileEntity> _history = new();
    private LinkedListNode<FileEntity>? _currentNode;

    public bool CanRedo => _currentNode?.Next is not null;
    public bool CanUndo => _currentNode?.Previous is not null;

    public FileEntity CurrentEntity
    {
        get => _currentNode?.Value ??
               throw new InvalidOperationException("Cannot get current entity from empty history");
        set
        {
            if (_currentNode?.Next is not null)
            {
                var trimCount = 0;
                var nodeToRemove = _currentNode.Next;

                while (nodeToRemove is not null)
                {
                    var nextNode = nodeToRemove.Next;
                    _history.Remove(nodeToRemove);
                    nodeToRemove = nextNode;
                    trimCount++;
                }

                logger.LogDebug("History trimmed, removed {Count} future states", trimCount);
            }

            var newNode = new LinkedListNode<FileEntity>(value);

            if (_history.Count == 0)
                _history.AddFirst(newNode);
            else
                _history.AddAfter(_currentNode, newNode);

            _currentNode = newNode;
            logger.LogDebug("Added new entity to history: {Path}", value.Path);
        }
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            logger.LogError("Cannot undo, history is empty or at the beginning");
            throw new InvalidOperationException(
                "Cannot undo operation. History is empty or current entity is the first one in history");
        }

        var previousEntity = _currentNode.Previous.Value;
        logger.LogDebug("Performing undo from {Current} to {Previous}", _currentNode.Value.Path, previousEntity.Path);
        _currentNode = _currentNode.Previous;
    }

    public void Redo()
    {
        if (!CanRedo)
        {
            logger.LogError("Cannot redo, no future states available");
            throw new InvalidOperationException(
                "Cannot redo operation. No future states available after current entity");
        }

        var nextEntity = _currentNode.Next.Value;
        logger.LogDebug("Performing redo from {Current} to {Next}", _currentNode.Value.Path, nextEntity.Path);
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

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}