namespace SolidZip.Core.Contracts.StateMachines;

public interface IExplorerStateMachine
{
    public bool CanUndo { get; set; }
    public bool CanRedo { get; set; }
    public FileEntity CurrentDirectory { get; set; }
    
    public ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity directory);
    public IconInfo GetIcon(string path);
    public void Redo();
    public void Undo();

}