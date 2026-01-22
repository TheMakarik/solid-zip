namespace SolidZip.Views.Converters;

[ValueConversion(typeof(string), typeof(ImageSource))]
public sealed class PathToImageSourceConvertor(
    IExplorerStateMachine explorer,
    PathsCollection paths,
    IOptions<ExplorerOptions> explorerOptions,
    IIconExtractorStateMachine iconExtractorStateMachine,
    ExtensionIconExtractor extensionIconExtractor,
    AssociatedIconExtractor associatedIconExtractor,
    ILogger<PathToImageSourceConvertor> logger) : IValueConverter
{
    private readonly string SzIconPath = "pack://application:,,," + paths.IconPath;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            if (value is not string path)
                return new BitmapImage();

            path = Environment.ExpandEnvironmentVariables(path);
            if (Enum.TryParse<FileSystemState>(parameter?.ToString() ?? string.Empty, out var state))
                return CreateIconFromState(state, path);

            if (path == explorerOptions.Value.RootDirectory)
                return CreateImageFromApplicationIcon();
            return path.StartsWith(explorerOptions.Value.RootDirectory)
                ? ExtractIcon(path.Substring(explorerOptions.Value.RootDirectory.Length))
                : ExtractIcon(path);
        }
        catch (Exception e)
        {
            logger.LogError("Cannot extract icon due to exception: {exception}", e.Message);
            return new BitmapImage();
        }
    }


    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private object? CreateIconFromState(FileSystemState state, string path)
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

    private ImageSource ExtractIcon(string path)
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