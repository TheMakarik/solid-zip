namespace SolidZip.ViewModels;

public sealed partial class FileCreationViewModel : ViewModelBase
{
    private readonly string _currentDirectory;
    private readonly IDialogHelper _dialogHelper;
    private readonly IExplorerStateMachine _explorer;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [FileDoNotExist(nameof(_currentDirectory))]
    [CanCreateItem]
    [NotifyCanExecuteChangedFor(nameof(CreateFileCommand))]
    [NotifyPropertyChangedFor(nameof(CanCreateFile))]
    private string _fileName;

    public FileCreationViewModel(StrongTypedLocalizationManager localization, IMessenger messenger,
        IExplorerStateMachine explorer, IDialogHelper dialogHelper) : base(localization, messenger)
    {
        messenger.RegisterAll(this);
        _currentDirectory = messenger.Send(new GetCurrentDirectory()).Response;
        _explorer = explorer;
        _messenger = messenger;
        _dialogHelper = dialogHelper;
        _fileName = localization.NewFile;
    }

    public bool CanCreateFile => !HasErrors;

    [RelayCommand(CanExecute = nameof(CanCreateFile))]
    private void CreateFile()
    {
        if (HasErrors)
            return;

        var path = Path.Combine(_currentDirectory, _fileName);
        File.Create(path).Close();
        _messenger.Send(new AddToTheCurrentDirectoryContent(path.ToFileEntity()));
        _dialogHelper.Close(ApplicationViews.CreateFile);
    }
}