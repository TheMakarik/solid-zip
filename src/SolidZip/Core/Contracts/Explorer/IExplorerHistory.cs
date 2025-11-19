namespace SolidZip.Core.Contracts.Explorer;

public interface IExplorerHistory : IEnumerable<FileEntity>
{
    public bool CanRedo { get; }
    public bool CanUndo { get; }
    public FileEntity CurrentEntity { get; set; }
    public void Undo();
    public void Redo();
}