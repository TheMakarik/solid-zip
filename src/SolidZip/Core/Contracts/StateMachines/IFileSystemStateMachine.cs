namespace SolidZip.Core.Contracts.StateMachines;

public interface IFileSystemStateMachine
{
    public void AttemptToSwitchState(string path);
    public FileSystemState GetState();
}