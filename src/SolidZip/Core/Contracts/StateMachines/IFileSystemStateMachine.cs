namespace SolidZip.Core.Contracts.StateMachines;

public interface IFileSystemStateMachine
{
    public void AttemptToSwitchState(string path, out IArchiveReader? reader);
    public FileSystemState GetState();
}