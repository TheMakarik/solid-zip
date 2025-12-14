namespace SolidZip.Core.Contracts.StateMachines;

public interface IExplorerStateMachine
{
    public bool CanUndo { get;  }
    public bool CanRedo { get;  }
    public ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity directory, bool addToHistory = true);
    public IconInfo GetIcon(string path, ExplorerState? state = null );
    public FileEntity Redo();
    public FileEntity Undo();
    public void BeginSearch();
    public FileEntity Search(string path, string pattern);
    public void EndSearch();

}