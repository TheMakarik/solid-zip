namespace SolidZip.Modules.StateMachines;

public class ItemsCreatorStateMachine(ILogger<ItemsCreatorStateMachine> logger, IFileSystemStateMachine stateMachine, IOptions<ExplorerOptions> options) : IItemsCreatorStateMachine
{
    public bool CanCreateItemsHere(string path) => Directory.Exists(path.CutPrefix(options.Value.RootDirectory));

    public void  CreateFile(string path)
    {
        if (stateMachine.GetState() == FileSystemState.Directory)
        {
            File.Create(path).Dispose();
            logger.LogInformation("File created {name}", path);
        }
    }

    public void CreateFolder(string path)
    {
        if (stateMachine.GetState() == FileSystemState.Directory)
        {
            Directory.CreateDirectory(path);
            logger.LogInformation("Directory created {name}", path);
        }
    }
}