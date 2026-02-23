namespace SolidZip.ViewModels;

public sealed partial class ExtractArchiveViewModel : ViewModelBase
{
    [ObservableProperty]
    [DirectoryExists] 
    private string _extractPath;

    [ObservableProperty] private bool _extractFullPath = true;
    [ObservableProperty] private bool _override = false;
    [ObservableProperty] private bool _createDirectory = true;
    [ObservableProperty] private bool _preserveAttributes = true;
    [ObservableProperty] private bool _preserveTime = true;
    [ObservableProperty] private string _extractedArchiveDirectoryName;
    
    private readonly IMessenger _messenger;
    private readonly WindowsExplorer _windowsExplorer;
    private readonly ILogger<ExtractArchiveViewModel> _logger;
    private readonly IExplorerStateMachine _explorerStateMachine;
    private readonly IDialogHelper _dialogHelper;

    public ExtractArchiveViewModel(StrongTypedLocalizationManager localization,
        IDialogHelper dialogHelper,
        ILogger<ExtractArchiveViewModel> logger,
        IExplorerStateMachine explorerStateMachine,
        WindowsExplorer windowsExplorer, 
        IMessenger messenger) : base(localization, messenger)
    {
        messenger.RegisterAll(this);
        
        _messenger = messenger;
        _dialogHelper = dialogHelper;
        _explorerStateMachine =  explorerStateMachine;
        _windowsExplorer = windowsExplorer;
        _logger = logger;
        
        var archivePath = _messenger.Send<GetCurrentDirectoryMessage>().Response.CutFromEnd(Path.DirectorySeparatorChar, '.');
        ExtractPath = GetExtractPath(archivePath);
        
        ExtractedArchiveDirectoryName = Path.GetFileNameWithoutExtension(archivePath);
    }

    [RelayCommand]
    private void GetExtractPathFromWindowsExplorer()
    {
        _logger.LogInformation("Start getting extract path from Windows Explorer");
        var result = _windowsExplorer.SelectFolder();
        if(result.Is(WindowsExplorerDialogResult.Ok))
            ExtractPath = result.Value ?? ExtractPath;
    }

    [RelayCommand]
    private async Task ExtractArchive()
    {
        var extractingOptions = new ArchiveExtractingOptions()
        {
            CreateExtractionDirectory = CreateDirectory,
            Override = Override,
            PreserveAttributes = PreserveAttributes,
            PreserveFileTime = PreserveTime,
            ExtractFullPath = ExtractFullPath,
            ExtractedArchiveDirectoryName =  ExtractedArchiveDirectoryName
        };

        var progress = new Progress<double>();
        _dialogHelper.ShowNonBlocking(ApplicationViews.Progress);
        var token = _messenger.Send(new GetCancellationTokenMessage()).Response.Token;
        _messenger.Send(new SetProgressMessage(base.Localization.ExtractingMessage));
        
        progress.ProgressChanged += (_, newProgressPercent)
            => _messenger.Send(new UpdateStartupProgressMessage(newProgressPercent));
        
        var reader = _explorerStateMachine.GetArchiveReader() ?? throw new InvalidOperationException("Cannot extract from null archive reader");
        await reader.ExtractAll(ExtractPath, progress,  extractingOptions, token);
        
        _dialogHelper.Close(ApplicationViews.Progress);
    }
    
    
    private string GetExtractPath(string archivePath)
    {
        var result =  Path.GetDirectoryName(archivePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(result);
        return result;
    }
}