using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using SolidZip.Model.Entities;
using SolidZip.Model.Enums;
using SolidZip.Model.Options;
using SolidZip.Services.ExplorerServices.Abstractions;

namespace SolidZip.ViewModels;

public partial class MainViewModel(IExplorer explorer, IOptions<ExplorerOptions> explorerOptions) : ViewModelBase
{

    [ObservableProperty] private string _currentDirectory = explorerOptions.Value.RootDirectory;
    
    
    [ObservableProperty] private ObservableCollection<FileEntity> _currentDirectoryContent =
        new ObservableCollection<FileEntity>(explorer
            .GetDirectoryContent(new FileEntity(explorerOptions.Value.RootDirectory, true)).Entities);
        
    
    [RelayCommand]
    private void GetDirectoryContent(FileEntity entity)
    {
        var explorerResult = explorer.GetDirectoryContent(entity);

        if (explorerResult.Result == ExplorerResult.Success)
        {
            CurrentDirectoryContent.Clear();
            foreach (var explorerResultEntity in explorerResult.Entities)
            {
                CurrentDirectoryContent.Add(explorerResultEntity);
            }
        }
           
    }
}