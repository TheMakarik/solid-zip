namespace SolidZip.Modules.StateMachines;

public class FileSystemStateMachine(ArchiveReaderFactory factory, ILogger<FileSystemStateMachine> logger)
    : IFileSystemStateMachine
{
    private readonly object @lock = new();
    private FileSystemState _currentState = FileSystemState.Directory;

    public void AttemptToSwitchState(string path, out IArchiveReader? reader)
    {
        if (_currentState == FileSystemState.Directory)
        {
            AttemptToSwitchStateToArchive(path, out reader);
        }
        else
        {
            reader = null;
            AttemptToSwitchStateToDirectory(path);
        }
    }

    public FileSystemState GetState()
    {
        return _currentState;
    }

    private void AttemptToSwitchStateToArchive(string path, out IArchiveReader? reader)
    {
        if (!CanChangeStateToArchive(path, out reader))
            return;

        logger.LogInformation("Switch file system state to archive, path: {path}", path);
        lock (@lock)
        {
            _currentState = FileSystemState.Archive;
        }
    }

    private void AttemptToSwitchStateToDirectory(string path)
    {
        if (!CanChangeStateToDirectory(path))
            return;

        logger.LogInformation("Switch file system state to directory, path: {path}", path);
        lock (@lock)
        {
            _currentState = FileSystemState.Directory;
        }
    }

    private bool CanChangeStateToDirectory(string path)
    {
        return Directory.Exists(path);
    }


    private bool CanChangeStateToArchive(string path, out IArchiveReader? reader)
    {
        if (Directory.Exists(path))
        {
            reader = null;
            return false;
        }


        var archivePath = path.CutFromEnd(Path.DirectorySeparatorChar, '.');
        var result = factory.TryGetFactory(archivePath, out reader);
        return result;
    }
}