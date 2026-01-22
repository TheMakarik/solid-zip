namespace SolidZip.ViewModels;

public partial class DirectoryCreationViewModel : ViewModelBase
{
    private readonly string _currentDirectory;
    private readonly IDialogHelper _dialogHelper;
    private readonly IExplorerStateMachine _explorer;
    private new readonly IMessenger _messenger;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [DirectoryDoNotExist(nameof(_currentDirectory))]
    [CanCreateItem]
    [NotifyCanExecuteChangedFor(nameof(CreateDirectoryCommand))]
    [NotifyPropertyChangedFor(nameof(CanCreateDirectory))]
    private string _directoryName;

    public DirectoryCreationViewModel(StrongTypedLocalizationManager localization, IMessenger messenger,
        IExplorerStateMachine explorer, IDialogHelper dialogHelper) : base(localization, messenger)
    {
        messenger.RegisterAll(this);
        _currentDirectory = messenger.Send(new GetCurrentDirectory()).Response;
        _explorer = explorer;
        _messenger = messenger;
        _dialogHelper = dialogHelper;
        _directoryName = localization.NewDirectory;
    }

    public bool CanCreateDirectory => !HasErrors;

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