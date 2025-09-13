using CommunityToolkit.Mvvm.Input;
using SolidZip.Localization;
using SolidZip.ViewModels.Messages;

namespace SolidZip.ViewModels;

public sealed partial class ExplorerViewModel(
    IExplorer explorer, 
    IOptions<ExplorerOptions> explorerOptions,
    StrongTypedLocalizationManager localizationManager)
    : ViewModelBase(localizationManager), IRecipient<FileEntityForGettingContentMessage>, IRecipient<CurrentDirectoryMessage>
{
    [ObservableProperty] private string _currentPath = explorerOptions.Value.RootDirectory;



    [RelayCommand]
    private void GetDirectoryContent(FileEntity directory)
    {
        
    }

    public void Receive(FileEntityForGettingContentMessage message)
    {
        GetDirectoryContent(message.Value);
    }

    public void Receive(CurrentDirectoryMessage message)
    {
        message.Reply(CurrentPath);
    }
}