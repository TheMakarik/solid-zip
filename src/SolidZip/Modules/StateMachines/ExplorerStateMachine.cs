namespace SolidZip.Modules.StateMachines;

public class ExplorerStateMachine(IExplorer explorer, IExplorerHistory explorerHistory) : IExplorerStateMachine
{
    private ExplorerState _state = ExplorerState.Directory;
    
    public bool CanUndo { get; set; }
    public bool CanRedo { get; set; }
    public FileEntity CurrentDirectory { get; set; }
    
    public async ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity directory)
    {
        throw new NotImplementedException();
    }
    
    public IconInfo GetIcon(string path)
    {
        throw new NotImplementedException();
    }

    public void Redo()
    {
       TryToUpdateState(explorerHistory.CurrentEntity.Path);
    }

    public void Undo()
    {
        TryToUpdateState(explorerHistory.CurrentEntity.Path);
    }

    private void TryToUpdateState(string path)
    {
        
    }
}