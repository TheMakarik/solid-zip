namespace SolidZip.Views.Converters;

[ValueConversion(typeof(string), typeof(ImageSource))]
public sealed class PathToImageSourceConvertor(
    IExplorerStateMachine explorer,
    PathsCollection paths,
    IOptions<ExplorerOptions> explorerOptions,
    IIconExtractorStateMachine iconExtractorStateMachine,
    ExtensionIconExtractor extensionIconExtractor,
    IArchiveSupportedExtensions archiveSupportedExtensions,
    AssociatedIconExtractor associatedIconExtractor,
    ILogger<PathToImageSourceConvertor> logger) : IValueConverter
{
    private readonly string SzIconPath = "pack://application:,,," + paths.IconPath;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            if (value is not FileEntity fileEntity)
               if(value is string path)
                   fileEntity = default(FileEntity) with { Path = path };
               else
                   return new BitmapImage();

            fileEntity = fileEntity with { Path = Environment.ExpandEnvironmentVariables(fileEntity.Path) };
            
            if (Enum.TryParse<FileSystemState>(parameter?.ToString() ?? string.Empty, out var state))
                return CreateIconFromState(state, fileEntity);

            if (ShouldUseApplicationIcon(fileEntity))
                return CreateImageFromApplicationIcon();
            return fileEntity.Path.StartsWith(explorerOptions.Value.RootDirectory)
                ? ExtractIcon(fileEntity with {Path = fileEntity.Path[explorerOptions.Value.RootDirectory.Length..]})
                : ExtractIcon(fileEntity);
        }
        catch (Exception e)
        {
            logger.LogError("Cannot extract icon due to exception: {exception}", e.Message);
            return new BitmapImage();
        }
    }

    private bool ShouldUseApplicationIcon(FileEntity fileEntity)
    {
        return fileEntity.Path == explorerOptions.Value.RootDirectory || archiveSupportedExtensions.Contains(fileEntity.Path);
    }


    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private object? CreateIconFromState(FileSystemState state, FileEntity path)
    {
        if (state == FileSystemState.Directory)
            return CreateIcon(associatedIconExtractor.Extract(path));
        return CreateIcon(extensionIconExtractor.Extract(path));
    }

    private ImageSource CreateImageFromApplicationIcon()
    {
        return CreateImageFromPath(SzIconPath);
    }

    private ImageSource CreateImageFromPath(string path)
    {
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(path, UriKind.Absolute);
        bitmapImage.EndInit();
        bitmapImage.Freeze();
        return bitmapImage;
    }

    private ImageSource ExtractIcon(FileEntity path)
    {
        using var iconInfo = iconExtractorStateMachine.ExtractIcon(path);

        return iconInfo.HIcon == 0
            ? new BitmapImage()
            : CreateIcon(iconInfo);
    }

    private ImageSource CreateIcon(IconInfo iconInfo)
    {
        var bitmapSource = Imaging.CreateBitmapSourceFromHIcon(
            iconInfo.HIcon,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        bitmapSource.Freeze();
        return bitmapSource;
    }
}