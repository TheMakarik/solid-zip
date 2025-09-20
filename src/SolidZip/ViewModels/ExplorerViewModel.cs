namespace SolidZip.ViewModels;

public sealed partial class ExplorerViewModel 
    : ViewModelBase, IRecipient<UpdateDirectoryContentRequestMessage>
{

    private const string GettingDirectoryContentResultCountLogMessage = "Getting content from {path} count: {count} and result {result}";
    private const string GettingDirectoryContentValues = "Getting content from {path}: {values} and result {result}";
    
    private readonly ILogger<ExplorerViewModel> _logger;
    private readonly IExplorer _explorer;
    private readonly IMessenger _messenger;
    private readonly IOptions<ExplorerOptions> _explorerOptions;

    [ObservableProperty] private ObservableCollection<FileEntity> _entities = new();
    [ObservableProperty] private FileEntity _selectedEntity;

    public ExplorerViewModel(IExplorer explorer, 
        IMessenger messenger,
        IOptions<ExplorerOptions> explorerOptions,
        ILogger<ExplorerViewModel> logger,
        StrongTypedLocalizationManager localizationManager) : base(localizationManager)
    {
        _explorer = explorer;
        _messenger = messenger;
        _logger = logger;
        _explorerOptions = explorerOptions;
        
        _messenger.RegisterAll(this);
        
        Task.Run(() =>
        {
            var rootDirectoryContent = GetDirectoryContent(
                new FileEntity(_explorerOptions.Value.RootDirectory, IsDirectory: true));
            
            rootDirectoryContent.Entities = rootDirectoryContent.Entities.Select(d =>
                new FileEntity(Environment.ExpandEnvironmentVariables(d.Path), true));
            FillDirectoryContent(rootDirectoryContent.Entities, explorerOptions.Value.RootDirectory);
        });
    }
    
    public void Receive(UpdateDirectoryContentRequestMessage message)
    {
        UpdateDirectoryContent(message.Directory);
    }
    
    [RelayCommand] 
    private void UpdateDirectoryContent(FileEntity entity)
    {
        if (entity.Path == _explorerOptions.Value.DeeperDirectoryName)
            GetDeeperDirectoryContent();
        else
        {
            var result = _explorer.GetDirectoryContent(entity);
            FillDirectoryContent(result.Entities, entity.Path);
        
            _messenger
                .Send(new UpdateCurrentDirectoryMessage(entity.Path));
        }
    }

    private void GetDeeperDirectoryContent()
    {
        var path = _messenger.Send(new GetCurrentDirectoryPathMessage()).Response;
        UpdateDirectoryContent(
            IsLogicalDrive(path)
                ? new FileEntity(_explorerOptions.Value.RootDirectory, IsDirectory: true)
                : new FileEntity(Path.GetDirectoryName(path)!, IsDirectory: true));
    }
    
    private (IEnumerable<FileEntity> Entities, ExplorerResult Result) GetDirectoryContent(FileEntity directory)
    {
        var result =  _explorer.GetDirectoryContent(directory);
        LogDirectoryContentCount(directory, result);
        _logger.LogDebug(GettingDirectoryContentValues, directory.Path, result.Entities, result.Result);
        return result;
    }

    private void LogDirectoryContentCount(FileEntity directory, (IEnumerable<FileEntity> Entities, ExplorerResult Result) result)
    {
        var counter = 0;
        result.Entities.ForEach(el => counter++);
        _logger.LogInformation(GettingDirectoryContentResultCountLogMessage, directory.Path, counter, result.Result);
    }

    private void FillDirectoryContent(IEnumerable<FileEntity> values, string path)
    {
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            Entities.Clear();
            AddDeeperDirectory(path);
            foreach (var entity in values)
                Entities.Add(entity);
        });
    }

    private bool IsLogicalDrive(string path)
    {
        return path is [_, ':', '\\'];
    }
    
    private void AddDeeperDirectory(string path)
    {
        if(path != _explorerOptions.Value.RootDirectory)
            Entities.Add(new FileEntity(_explorerOptions.Value.DeeperDirectoryName, IsDirectory: true));
    }

 
}