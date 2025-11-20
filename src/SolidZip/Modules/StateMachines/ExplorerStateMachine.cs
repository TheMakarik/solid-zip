using SolidZip.Factories;

namespace SolidZip.Modules.StateMachines;

public class ExplorerStateMachine(
    IExplorer explorer, 
    AssociatedIconExtractor iconExtractor,
    ExtensionIconExtractor archiveContentIconExtractor,
    IExplorerHistory explorerHistory, 
    ArchiveReaderFactory factory) : IExplorerStateMachine
{
    private ExplorerState _state = ExplorerState.Directory;
    private IArchiveReader? _archiveReader;
    private string? _archivePath;
    
    public bool CanUndo { get; set; }
    public bool CanRedo { get; set; }
    public FileEntity CurrentDirectory { get; set; }
    
    public async ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity directory)
    {
        throw new NotImplementedException();
    }
    
    public IconInfo GetIcon(string path)
    {
        if (_state == ExplorerState.Directory)
            return iconExtractor.Extract(path);
        return archiveContentIconExtractor.Extract(path.CutFromEnd(charTillCut: '\\',  stopChar: '.'));
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
        if (CanChangeStateToArchive(path, out var result))
        {
            _archiveReader = result;
            _archivePath = path;
            _state = ExplorerState.Archive;
        }
        
        if(_state == ExplorerState.Directory)
            _archiveReader?.Dispose();
    }

    private bool CanChangeStateToArchive(string path, out IArchiveReader? result)
    {
        IArchiveReader? reader = null;
        var canChange =   ((!Directory.Exists(path) 
                            || _state == ExplorerState.Directory)
                            && factory.TryGetFactory(path, out reader));
        result = reader;
        return canChange;
    }
    
    
}