namespace SolidZip.Core.Contracts.StateMachines;

public interface IItemsCreatorStateMachine
{
    public bool CanCreateItemsHere(string path);
    public void CreateFile(string path);
    public void CreateFolder(string path);
}