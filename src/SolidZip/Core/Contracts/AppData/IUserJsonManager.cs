namespace SolidZip.Core.Contracts.AppData;

public interface IUserJsonManager
{
    public void ChangeThemeName(string newThemeName);
    public  void ChangeCurrentCulture(CultureInfo newCurrentCulture);
    public void ChangeAttachPluginsConsole(bool attach);
    public  void ChangeExplorerElementsView(ExplorerElementsView newExplorerElementsView);
    public  void ChangeFileSizeMeasurement(FileSizeMeasurement newFileSizeMeasurement);
    public  void ChangeRootDirectoryAdditionalContent(IEnumerable<string> newContent);
    public  void ChangeShowHiddenDirectories(bool showHidden);
    public ValueTask<CultureInfo> GetCurrentCultureAsync();
    public ValueTask<bool> GetAttachPluginsConsoleAsync();
    public ValueTask<ExplorerElementsView> GetExplorerElementsViewAsync();
    public ValueTask<string> GetThemeNameAsync();
    public ValueTask<FileSizeMeasurement> GetFileSizeMeasurementAsync();
    public  ValueTask<IEnumerable<string>> GetRootDirectoryAdditionalContentAsync();
    public  ValueTask<bool> GetShowHiddenDirectoriesAsync();
}