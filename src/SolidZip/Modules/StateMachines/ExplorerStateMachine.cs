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
    
    public async ValueTask<Result<ExplorerResult, IEnumerable<FileEntity>>> GetContentAsync(FileEntity directory, bool addToHistory = true)
    {
        var result = _state == ExplorerState.Directory 
            ? await explorer.GetDirectoryContentAsync(directory)
            : _archiveReader!.GetEntries(directory);
        TryToUpdateState(directory.Path);
        
        if(addToHistory)
             explorerHistory.CurrentEntity = directory;
        return result;
    }
    
    public IconInfo GetIcon(string path)
    {
        return _state == ExplorerState.Directory 
            ? iconExtractor.Extract(path) 
            : archiveContentIconExtractor.Extract(path.GetExtensionFromEnd());
    }

    public FileEntity Redo()
    { 
        explorerHistory.Redo();
        return explorerHistory.CurrentEntity;
    }

    public FileEntity Undo()
    {
        explorerHistory.Undo();
        return explorerHistory.CurrentEntity;
    }

    private void TryToUpdateState(string path)
    {
        if (CanChangeStateToArchive(path, out var result))
        {
            _archiveReader = result;
            _archivePath = path;
            _state = ExplorerState.Archive;
        }

        if (CanChangeStateToDirectory(path))
            _state = ExplorerState.Archive;
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