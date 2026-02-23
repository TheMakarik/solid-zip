namespace SolidZip.ViewModels;

public partial class ExtractArchiveViewModel : ViewModelBase
{
    [ObservableProperty] private string _extractPath;
    
    private readonly IMessenger _messenger;

    public ExtractArchiveViewModel(StrongTypedLocalizationManager localization, IMessenger messenger) : base(localization, messenger)
    {
        messenger.RegisterAll(this);
        
        _messenger = messenger; 
        _extractPath = GetOutputDirectory();
    }

    private string GetOutputDirectory()
    {
        var archivePath = _messenger.Send<GetCurrentDirectory>().Response.CutFromEnd(Path.DirectorySeparatorChar, '.');
        var result =  Path.GetDirectoryName(archivePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(result);
        return result;
    }
}