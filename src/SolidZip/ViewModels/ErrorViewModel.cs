namespace SolidZip.ViewModels;

public partial class ErrorViewModel(
    PathsCollection paths,
    IOptions<TheMakariksOptions> theMakariksOptions,
    WindowsExplorer explorer,
    StrongTypedLocalizationManager localization,
    IMessenger messenger) : ViewModelBase(localization, messenger)
{

    [RelayCommand]
    private async Task ShowLogs()
    {
        await Task.Run(() =>
        {
            explorer.OpenFolder(paths.Logging);
        });
    }

    [RelayCommand]
    private async Task ContactToTheSupport()
    {
        await Task.Run(() =>
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = theMakariksOptions.Value.TelegramLink,
                UseShellExecute = true
            };
        
            Process.Start(processInfo);
        });
    }
}