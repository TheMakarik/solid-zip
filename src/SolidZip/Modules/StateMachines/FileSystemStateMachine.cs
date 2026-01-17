namespace SolidZip.Modules.StateMachines;

public class FileSystemStateMachine(ArchiveReaderFactory factory, ILogger<FileSystemStateMachine> logger) : IFileSystemStateMachine
{
    private FileSystemState _currentState = FileSystemState.Directory;
    
    public void AttemptToSwitchState(string path)
    {
        if (_currentState == FileSystemState.Directory)
            AttemptToSwitchStateToArchive(path);
        else
            AttemptToSwitchStateToDirectory(path);

    }
    
    public FileSystemState GetState()
    {
       return _currentState;
    }

    private void AttemptToSwitchStateToArchive(string path)
    {
        if (!CanChangeStateToArchive(path))
            return;
        
        logger.LogInformation("Switch file system state to archive, path: {path}", path);
        _currentState =  FileSystemState.Archive;
    }
    

    private void AttemptToSwitchStateToDirectory(string path)
    {
        if (!CanChangeStateToDirectory(path))
            return;
        
        logger.LogInformation("Switch file system state to directory, path: {path}", path);
        _currentState =  FileSystemState.Directory;
    }

    private bool CanChangeStateToDirectory(string path)
    {
        return Directory.Exists(path);
    }


    private bool CanChangeStateToArchive(string path)
    {
        if (Directory.Exists(path))
            return false;

        var archivePath = path.CutFromEnd(Path.DirectorySeparatorChar, '.');
        var result = factory.TryGetFactory(archivePath, out var reader);
        reader?.Dispose();;
        
        return result;
    }
    
}