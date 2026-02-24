
namespace SolidZip.ViewModels;

public partial class ZipArchiveCreatorViewModel : ViewModelBase
{
    private readonly StrongTypedLocalizationManager _localization;
    private readonly ILuaUiData _luaUiData;
    private readonly IMessenger _messanger;
    private readonly WindowsExplorer _windowsExplorer;
    
    [ObservableProperty] private string _archiveName;
    [ObservableProperty] private string _archivePath;
    [ObservableProperty] private ZipEncryption _encryption = ZipEncryption.None;
    [ObservableProperty] private ObservableCollection<string> _filesToAdd;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private bool _isPasswordEnabled;
    [ObservableProperty] private ObservableCollection<ZipEncryption> _encryptionOptions;

    public ZipArchiveCreatorViewModel(
        StrongTypedLocalizationManager localization, 
        ILuaUiData luaUiData,
        IMessenger messenger,
        WindowsExplorer windowsExplorer) : base(localization, messenger)
    {
        _localization = localization;
        _luaUiData = luaUiData;
        _messanger = messenger;
        _windowsExplorer = windowsExplorer;
        
        EncryptionOptions = 
        [
            ZipEncryption.None,
            ZipEncryption.Aes156,
            ZipEncryption.Aes256
        ];
        _archiveName = localization.NewZip;
        _archivePath = _messanger.Send(new GetCurrentDirectoryMessage()).Response;
        _filesToAdd = messenger.Send(new GetSelectedFileEntitiesMessage()).Response.Select(e => e.Path).ToObservable();
        messenger.RegisterAll(this);
    }

    [RelayCommand]
    private void SelectOutputFolder()
    {
        var result = _windowsExplorer.SelectFolder();
        
        if (!result.Is(WindowsExplorerDialogResult.Ok))
            return;
            
        ArchivePath = result.Value!;
        _luaUiData.AddOrUpdate("archive_path", ArchivePath);
        
        if (string.IsNullOrWhiteSpace(ArchiveName))
        {
            ArchiveName = $"Archive_{DateTime.Now:yyyyMMdd_HHmmss}";
            _luaUiData.AddOrUpdate("archive_name", ArchiveName);
        }
    }

    [RelayCommand]
    private void AddFiles()
    {
        var result = _windowsExplorer.SelectPaths();
        
        if (!result.Is(WindowsExplorerDialogResult.Ok))
            return;
            
        foreach (var file in result.Value ?? [])
        {
            if (!FilesToAdd.Contains(file))
                FilesToAdd.Add(file);
        }
        
        _luaUiData.AddOrUpdate("files_to_add_count", FilesToAdd.Count);
        _luaUiData.AddOrUpdate("files_to_add", FilesToAdd.ToArray());
    }

    [RelayCommand]
    private void AddFolder()
    {
        var result = _windowsExplorer.SelectFolder();
        
        if (!result.Is(WindowsExplorerDialogResult.Ok))
            return;
            
        if (!FilesToAdd.Contains(result.Value!))
            FilesToAdd.Add(result.Value!);
            
        _luaUiData.AddOrUpdate("files_to_add_count", FilesToAdd.Count);
        _luaUiData.AddOrUpdate("files_to_add", FilesToAdd.ToArray());
    }

    [RelayCommand]
    private void RemoveFile(string filePath)
    {
        FilesToAdd.Remove(filePath);
        _luaUiData.AddOrUpdate("files_to_add_count", FilesToAdd.Count);
        _luaUiData.AddOrUpdate("files_to_add", FilesToAdd.ToArray());
    }

    [RelayCommand]
    private void ClearFiles()
    {
        FilesToAdd.Clear();
        _luaUiData.AddOrUpdate("files_to_add_count", 0);
        _luaUiData.AddOrUpdate("files_to_add", Array.Empty<string>());
    }

    [RelayCommand]
    private void CreateZip()
    {
        // Пустой метод для дальнейшей реализации
    }

    partial void OnEncryptionChanged(ZipEncryption value)
    {
        IsPasswordEnabled = value != ZipEncryption.None;
        _luaUiData.AddOrUpdate("encryption", value.ToString());
        
        if (!IsPasswordEnabled)
        {
            Password = string.Empty;
            _luaUiData.AddOrUpdate("password", string.Empty);
        }
    }

    partial void OnArchiveNameChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _luaUiData.AddOrUpdate("archive_name", value);
    }

    partial void OnArchivePathChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _luaUiData.AddOrUpdate("archive_path", value);
    }

    partial void OnPasswordChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _luaUiData.AddOrUpdate("password_set", true);
    }
}