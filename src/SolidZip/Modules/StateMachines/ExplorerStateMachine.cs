namespace SolidZip.Modules.StateMachines;

public sealed class ExplorerStateMachine(
    IExplorer explorer,
    ArchiveReaderFactory factory,
    IExplorerHistory history,
    IFileSystemStateMachine stateMachine) : IExplorerStateMachine
{
    private IArchiveReader? _beforeUpdateReader;
    public bool CanUndo => history.CanUndo;
    public bool CanRedo => history.CanRedo;

    public async ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity entity,
        bool addToHistory = true)
    {
        stateMachine.AttemptToSwitchState(entity.Path, out var reader);
        var result = stateMachine.GetState() == FileSystemState.Directory
            ? await explorer.GetDirectoryContentAsync(entity)
            : (reader ?? _beforeUpdateReader)?.GetEntries(entity)
              ?? throw new InvalidOperationException("Cannot get content of empty archive reader");

        if (addToHistory && result.Is(ExplorerResult.Success))
            history.CurrentEntity = entity;
        _beforeUpdateReader = reader ?? _beforeUpdateReader;
        return result;
    }

    public FileEntity Undo()
    {
        history.Undo();
        return history.CurrentEntity;
    }

    public FileEntity Redo()
    {
        history.Redo();
        return history.CurrentEntity;
    }
}