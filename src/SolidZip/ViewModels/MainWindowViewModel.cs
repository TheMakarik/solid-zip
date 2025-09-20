namespace SolidZip.ViewModels;

public sealed partial class MainWindowViewModel 
    : ViewModelBase,
    IRecipient<ShowUnauthorizedAccessMessage>,
    IRecipient<UpdateCurrentDirectoryMessage>,
    IRecipient<GetCurrentDirectoryPathMessage>
{
    private readonly IMessenger _messenger;
    private readonly ViewModelLocator _locator;

    [ObservableProperty] private UserControl _explorerControl;
    [ObservableProperty] private string _currentPath;

    public MainWindowViewModel(
        IOptions<ExplorerOptions> explorerOptions,
        IMessenger messenger,
        ViewModelLocator locator,
        StrongTypedLocalizationManager localizationManager) : base(localizationManager)
    {
        _messenger = messenger;
        _locator = locator;
        
        _messenger.RegisterAll(this);
        ExplorerControl = _locator.GetView<ListExplorerItemsView>();
        
        _messenger.Send(new UpdateDirectoryContentRequestMessage
        {
            Directory = new(explorerOptions.Value.RootDirectory, IsDirectory: true)
        });
    }

    public void Receive(ShowUnauthorizedAccessMessage message)
    {
        
    }

    public void Receive(UpdateCurrentDirectoryMessage message)
    {
        CurrentPath = message.Value;
    }

    public void Receive(GetCurrentDirectoryPathMessage message)
    {
        message.Reply(CurrentPath);
    }
}