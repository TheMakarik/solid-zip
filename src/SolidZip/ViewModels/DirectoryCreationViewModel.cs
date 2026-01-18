namespace SolidZip.ViewModels;

public partial class DirectoryCreationViewModel : ViewModelBase
{
    private readonly string _currentDirectory;
    private new readonly IMessenger _messenger;
    private readonly IExplorerStateMachine _explorer;
    private readonly IDialogHelper _dialogHelper;

    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [DirectoryDoNotExist(nameof(_currentDirectory))]
    [CanCreateItem]
    [NotifyCanExecuteChangedFor(nameof(CreateDirectoryCommand))]
    [NotifyPropertyChangedFor(nameof(CanCreateDirectory))]
    private string _directoryName;

    public bool CanCreateDirectory => !HasErrors;

    public DirectoryCreationViewModel(StrongTypedLocalizationManager localization, IMessenger messenger, IExplorerStateMachine explorer, IDialogHelper dialogHelper) : base(localization, messenger)
    {
        messenger.RegisterAll(this);
        _currentDirectory = messenger.Send(new GetCurrentDirectory()).Response;
        _explorer = explorer;
        _messenger = messenger;
        _dialogHelper = dialogHelper;
        _directoryName = localization.NewDirectory;
    }

    [RelayCommand(CanExecute = nameof(CanCreateDirectory))]
    private void CreateDirectory()
    {
        if (HasErrors) 
            return;
        
        var path = Path.Combine(_currentDirectory, _directoryName);
        Directory.CreateDirectory(path);
        _messenger.Send(new AddToTheCurrentDirectoryContent(path.ToDirectoryFileEntity()));
        _dialogHelper.Close(ApplicationViews.CreateFolder);
    }
}