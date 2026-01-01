namespace SolidZip.ViewModels;

public partial class DirectoryCreationViewModel : ViewModelBase
{
    private readonly string _currentDirectory;
    private new readonly IMessenger _messenger;

    [ObservableProperty] private string _directoryName;

    public DirectoryCreationViewModel(StrongTypedLocalizationManager localization, IMessenger messenger) : base(localization, messenger)
    {
        messenger.RegisterAll(this);
        _currentDirectory = messenger.Send(new GetCurrentDirectory()).Response;

        _messenger = messenger;
    }

    [RelayCommand]
    private void CreateDirectory()
    {
        var directoryPath = Path.Combine(_currentDirectory, _directoryName); 
        Directory.CreateDirectory(directoryPath);
        _messenger.Send(new AddToTheCurrentDirectoryContent(directoryPath.ToDirectoryFileEntity()));
    }

    public ValidationResult DirectoryNotExists(string name)
    {
        return Directory.Exists(Path.Combine(_currentDirectory, name)) 
            ? new ValidationResult(isValid: false, "Directory already exists") 
            : new ValidationResult(isValid: true, "Directory not exists");
    }
}