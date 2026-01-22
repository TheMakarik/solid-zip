namespace SolidZip.Core.Contracts.StateMachines;

public interface IExplorerStateMachine
{
    public bool CanUndo { get; }
    public bool CanRedo { get; }

    public ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity entity,
        bool addToHistory = true);

    public FileEntity Undo();
    public FileEntity Redo();
}