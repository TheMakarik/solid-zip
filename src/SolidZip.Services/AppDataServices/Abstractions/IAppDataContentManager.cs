namespace SolidZip.Services.AppDataServices.Abstractions;

public interface IAppDataContentManager
{
    public Task ChangeThemeNameAsync(string newThemeName);
    public Task ChangeCurrentCultureAsync(CultureInfo newCurrentCulture);
    public Task ChangeExplorerElementsView(ExplorerElementsView newExplorerElementsView);
    public Task ChangeUseCustomIconsCollectionAsync(bool newUseCustomIconsCollection);

    public Task<CultureInfo> GetCurrentCultureAsync();
    public Task<ExplorerElementsView> GetExplorerElementsViewAsync();
    public Task<bool> GetUseCustomIconsCollectionAsync();
    public Task<string> GetThemeNameAsync();
}