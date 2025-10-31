namespace SolidZip.Services.AppDataServices.Abstractions;

public interface IAppDataContentManager
{
    public Task ChangeThemeNameAsync(string newThemeName);
    public Task ChangeCurrentCultureAsync(CultureInfo newCurrentCulture);
    public Task ChangeExplorerElementsViewAsync(ExplorerElementsView newExplorerElementsView);
    
    public ValueTask<CultureInfo> GetCurrentCultureAsync();
    public ValueTask<ExplorerElementsView> GetExplorerElementsViewAsync();
    public ValueTask<string> GetThemeNameAsync();

}