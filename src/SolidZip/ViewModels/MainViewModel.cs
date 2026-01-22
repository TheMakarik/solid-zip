namespace SolidZip.ViewModels;

public sealed partial class MainViewModel : ViewModelBase,
    IRecipient<AddToTheCurrentDirectoryContent>,
    IRecipient<GetCurrentDirectory>,
    IRecipient<GetSelectedFileEntities>
{
    private const int ExplorerElementsHeightMax = 30;
    private const int ExplorerElementsHeightMin = 15;
    private readonly IItemsCreatorStateMachine _creator;
    private readonly IDialogHelper _dialogHelper;

    private readonly IExplorerStateMachine _explorer;
    private readonly ILogger<MainViewModel> _logger;
    private readonly ILuaEventRaiser _raiser;
    private readonly ISearcherStateMachine _searcher;
    private readonly ILuaUiData _uiData;
    private readonly IUserJsonManager _userJsonManager;
    [ObservableProperty] private bool _canRedo;
    [ObservableProperty] private bool _canUndo;
    [ObservableProperty] private ObservableCollection<FileEntity> _currentExplorerContent = new();

    [ObservableProperty] private string _currentRealPath = string.Empty;
    [ObservableProperty] private string _currentUiPath = string.Empty;

    [ValueRange(ExplorerElementsHeightMin, ExplorerElementsHeightMax)] [ObservableProperty]
    private int _explorerElementsHeight;

    [ObservableProperty] private string _searchWatermark = string.Empty;
    [ObservableProperty] private ObservableCollection<FileEntity> _selectedFileEntities = [];


    public MainViewModel(
        ILogger<MainViewModel> logger,
        IMessenger messenger,
        ISearcherStateMachine searcher,
        IUserJsonManager userJsonManager,
        IExplorerStateMachine explorer,
        ILuaEventRaiser eventRaiser,
        ILuaUiData uiData,
        IItemsCreatorStateMachine creatorStateMachine,
        IDialogHelper dialogHelper,
        StrongTypedLocalizationManager localization,
        IOptions<ExplorerOptions> options) : base(localization, messenger)
    {
        _explorer = explorer;
        _canRedo = explorer.CanRedo;
        _logger = logger;
        _canUndo = explorer.CanUndo;
        _uiData = uiData;
        _creator = creatorStateMachine;
        _searcher = searcher;
        _raiser = eventRaiser;
        _userJsonManager = userJsonManager;
        _dialogHelper = dialogHelper;
        messenger.RegisterAll(this);
        var root = default(FileEntity) with { Path = options.Value.RootDirectory, IsDirectory = true };
        SelectedFileEntities.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SelectedFileEntities));
        GetContentAsync(root);
    }

    public void Receive(AddToTheCurrentDirectoryContent message)
    {
        CurrentExplorerContent.Add(message.Value);
        CurrentExplorerContent = CurrentExplorerContent
            .OrderBy(f => !f.IsDirectory)
            .ThenBy(f => f.Path)
            .ToObservable();
    }

    public void Receive(GetCurrentDirectory message)
    {
        message.Reply(CurrentRealPath);
    }

    public void Receive(GetSelectedFileEntities message)
    {
        message.Reply(SelectedFileEntities);
    }

    [RelayCommand]
    private async Task GetContentAsync(FileEntity directory)
    {
        if (!IsElementsHeightLoaded())
            await LoadElementsHeightAsync();

        var result = await _explorer.GetContentAsync(directory);
        await ValidateExplorerResultAsync(result, directory);
    }

    [RelayCommand]
    private async Task GetSearchWatermarkContentAsync()
    {
        if (CanChangeBackDirectory())
            await GetContentAsync(default(FileEntity) with { IsDirectory = true, Path = CurrentUiPath });
        else
            await GetContentAsync(default(FileEntity) with { IsDirectory = true, Path = SearchWatermark });
    }


    [RelayCommand]
    private void ChangeExplorerElementsHeight(int value)
    {
        if (ExplorerElementsHeight >= ExplorerElementsHeightMax && value > 0)
            return;

        if (ExplorerElementsHeight <= ExplorerElementsHeightMin && value < 0)
            return;

        ExplorerElementsHeight += value;
        _userJsonManager.ChangeExplorerElementsHeight(ExplorerElementsHeight);
        _logger.LogDebug("Changing ExplorerElementsHeight to {val}", ExplorerElementsHeight);
    }

    [RelayCommand]
    private void OpenSettings()
    {
        _dialogHelper.Show(ApplicationViews.Settings);
    }

    [RelayCommand]
    private async Task RedoAsync()
    {
        await Task.Run(async () =>
        {
            if (!CanRedo)
                return;

            var entity = _explorer.Redo();
            var directoryContent = await _explorer.GetContentAsync(entity, false);
            await ValidateExplorerResultAsync(directoryContent, entity);
            _raiser.RaiseBackground("redo_executed", new { entity, content = directoryContent.Value });
        });
    }

    [RelayCommand]
    private async Task UndoAsync()
    {
        await Task.Run(async () =>
        {
            if (!_explorer.CanUndo)
                return;

            var entity = _explorer.Undo();
            var directoryContent = await _explorer.GetContentAsync(entity, false);
            await ValidateExplorerResultAsync(directoryContent, entity);
            _raiser.RaiseBackground("undo_executed", new { entity, content = directoryContent.Value });
        });
    }

    [RelayCommand]
    private async Task StartSearchAsync()
    {
        _searcher.Begin();
        await SearchAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await Task.Run(async () =>
        {
            var result = _searcher.Search(CurrentRealPath, CurrentUiPath.Substring(CurrentUiPath.Length));
            await Application.Current.Dispatcher.InvokeAsync(() => SearchWatermark = result.Path);
        });
    }

    [RelayCommand]
    private void StopSearch()
    {
        _searcher.End();
    }

    [RelayCommand]
    private void NewZipArchive()
    {
        _dialogHelper.Show(ApplicationViews.NewZip);
    }

    [RelayCommand]
    private void NewDirectory()
    {
        if (_creator.CanCreateItemsHere(CurrentRealPath))
            _dialogHelper.Show(ApplicationViews.CreateFolder);
        else
            MessageBox.Show(Localization.CannotCreateDirectoryHere, Localization.NewDirectory, MessageBoxButton.OK);
    }

    [RelayCommand]
    private void NewFile()
    {
        if (_creator.CanCreateItemsHere(CurrentRealPath))
            _dialogHelper.Show(ApplicationViews.CreateFile);
        else
            MessageBox.Show(Localization.CannotCreateFileHere, Localization.NewDirectory, MessageBoxButton.OK);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CurrentUiPath):
                _uiData.AddOrUpdate("current_ui_path", CurrentUiPath);
                _raiser.RaiseBackground("current_ui_path_changed", new { current_ui_path = CurrentUiPath });
                break;
            case nameof(CurrentRealPath):
                _uiData.AddOrUpdate("current_real_path", CurrentRealPath);
                _raiser.RaiseBackground("current_real_path_changed", new { current_real_path = CurrentRealPath });
                if (IsSearching())
                {
                    SearchWatermark = string.Empty;
                    StopSearch();
                }

                break;
            case nameof(SearchWatermark):
                _uiData.AddOrUpdate("search_watermark", SearchWatermark);
                _raiser.RaiseBackground("search_watermark_changed", new { search_watermark = SearchCommand });
                break;
            case nameof(ExplorerElementsHeight):
                _uiData.AddOrUpdate("explorer_height", ExplorerElementsHeight);
                _raiser.RaiseBackground("explorer_height_changed", new { height = ExplorerElementsHeight });
                break;
            case nameof(SelectedFileEntities):
                var paths = SelectedFileEntities.Select(e => e.Path).ToArray();
                _uiData.AddOrUpdate("selected_entities_path", paths);
                _raiser.RaiseBackground("selected_entities_path_changed", new { paths });
                break;
        }

        base.OnPropertyChanged(e);
    }

    private bool IsSearching()
    {
        return string.IsNullOrWhiteSpace(SearchWatermark);
    }

    private async Task ValidateExplorerResultAsync(Result<ExplorerResult, IEnumerable<FileEntity>> result,
        FileEntity directory)
    {
        if (result.Is(ExplorerResult.Success))
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CurrentExplorerContent = result.Value?.ToObservable() ?? [];
                CurrentRealPath = directory.Path = Environment.ExpandEnvironmentVariables(directory.Path);
                CurrentUiPath = CurrentRealPath;
                CanUndo = _explorer.CanUndo;
                CanRedo = _explorer.CanRedo;
            });
    }

    private async Task LoadElementsHeightAsync()
    {
        ExplorerElementsHeight = await _userJsonManager.GetExplorerElementsHeightAsync();
    }

    private bool IsElementsHeightLoaded()
    {
        return ExplorerElementsHeight != 0;
    }

    private bool CanChangeBackDirectory()
    {
        return CurrentUiPath.Length < CurrentRealPath.Length && Directory.Exists(CurrentUiPath);
    }
}