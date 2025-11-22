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

    public bool CanUndo => explorerHistory.CanUndo;
    public bool CanRedo => explorerHistory.CanRedo;
    
    public async ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity directory)
    {
        var result = _state == ExplorerState.Directory 
            ? await explorer.GetDirectoryContentAsync(directory)
            : _archiveReader!.GetEntries(directory);
        return result;
    }
    
    public IconInfo GetIcon(string path)
    {
        return _state == ExplorerState.Directory 
            ? iconExtractor.Extract(path) 
            : archiveContentIconExtractor.Extract(path.GetExtensionFromEnd());
    }

    public void Redo()
    { 
        if(explorerHistory.CanRedo)
             explorerHistory.Redo();
        TryToUpdateState(explorerHistory.CurrentEntity.Path);
    }

    public void Undo()
    {
        if(explorerHistory.CanUndo)
            explorerHistory.Undo();
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
        
        if(CanChangeStateToDirectory(path))
        
        if(_state == ExplorerState.Directory)
            _archiveReader?.Dispose();
    }

    private bool CanChangeStateToDirectory(string path)
    {
        return _state == ExplorerState.Archive && Directory.Exists(path);
    }

    private bool CanChangeStateToArchive(string path, out IArchiveReader? result)
    {
        IArchiveReader? reader = null;
        var canChange =   ((!Directory.Exists(path) 
                            || _state == ExplorerState.Directory)
                            && factory.TryGetFactory(path.CutFromEnd('\\', '.'), out reader));
        result = reader;
        return canChange;
    }
    
    
}