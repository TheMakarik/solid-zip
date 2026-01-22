namespace SolidZip.ViewModels;

public sealed partial class ErrorViewModel : ViewModelBase
{
    private readonly WindowsExplorer _explorer;
    private readonly PathsCollection _paths;
    private readonly IOptions<TheMakariksOptions> _theMakariksOptions;

    public ErrorViewModel(PathsCollection paths,
        IOptions<TheMakariksOptions> theMakariksOptions,
        WindowsExplorer explorer,
        StrongTypedLocalizationManager localization,
        IMessenger messenger) : base(localization, messenger)
    {
        _paths = paths;
        _theMakariksOptions = theMakariksOptions;
        _explorer = explorer;

        messenger.RegisterAll(this);
    }

    [RelayCommand]
    private async Task ShowLogs()
    {
        await Task.Run(() => { _explorer.OpenFolder(_paths.Logging); });
    }

    [RelayCommand]
    private async Task ContactToTheSupport()
    {
        await Task.Run(() =>
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = _theMakariksOptions.Value.TelegramLink,
                UseShellExecute = true
            };

            Process.Start(processInfo);
        });
    }
}