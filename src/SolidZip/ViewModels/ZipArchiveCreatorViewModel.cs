namespace SolidZip.ViewModels;

public partial class ZipArchiveCreatorViewModel : ViewModelBase
{
    private readonly StrongTypedLocalizationManager _localization;

    private readonly ILuaUiData _luaUiData;
    private readonly IMessenger _messanger;
    [ObservableProperty] private string _archiveName;
    [ObservableProperty] private string _archivePath;
    [ObservableProperty] private ZipEncryption _encryption = ZipEncryption.None;
    [ObservableProperty] private ObservableCollection<string> _filesToAdd = [];


    public ZipArchiveCreatorViewModel(StrongTypedLocalizationManager localization, ILuaUiData luaUiData,
        IMessenger messenger) : base(localization, messenger)
    {
        _localization = localization;
        _luaUiData = luaUiData;
        _messanger = messenger;
        messenger.RegisterAll(this);
    }

    private ValidationResult FileNotExists(string path)
    {
        throw new Exception();
    }
}