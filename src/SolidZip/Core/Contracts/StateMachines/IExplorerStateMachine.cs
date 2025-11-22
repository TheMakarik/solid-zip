namespace SolidZip.Core.Contracts.StateMachines;

public interface IExplorerStateMachine
{
    public bool CanUndo { get;  }
    public bool CanRedo { get;  }
    public ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity directory);
    public IconInfo GetIcon(string path);
    public void Redo();
    public void Undo();

}