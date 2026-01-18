namespace SolidZip.Core.Contracts.StateMachines;

public interface ISearcherStateMachine
{
    public void Begin();
    public void End();
    public FileEntity Search(string path, string pattern);
}