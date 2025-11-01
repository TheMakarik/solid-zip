namespace SolidZip.Services.ExplorerServices.Abstractions;

public interface IExplorerFacade : IExplorer, IDirectorySearcher
{
   public void SetMode(ExplorerMode mode);
   public void StartSearching();
   public void StopSearching();

}