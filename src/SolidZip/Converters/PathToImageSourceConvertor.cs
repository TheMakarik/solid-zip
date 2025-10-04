using SolidZip.Converters.Abstractions;

namespace SolidZip.Converters;

[ValueConversion(typeof(string), typeof(ImageSource))]
public sealed class PathToImageSourceConvertor : OneWayConvertor
{
    private const string SzUndoPath = "pack://application:,,,/assets/szundo.png";
    private const string SzIconPath = "pack://application:,,,/assets/icon.ico";

    private const string CannotExtractIconDueToExceptionLogMessage =
        "Cannot extract icon due to exception: {exception}";
    
    private readonly IAssociatedIconExtractor _extractor = 
        Ioc.Default.GetRequiredService<IAssociatedIconExtractor>();
    private readonly IOptions<ExplorerOptions> _explorerOptions =
        Ioc.Default.GetRequiredService<IOptions<ExplorerOptions>>();
    private readonly ILogger<PathToImageSourceConvertor> _logger =
        Ioc.Default.GetRequiredService<ILogger<PathToImageSourceConvertor>>();
 
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            if (value is not string path)
                return new BitmapImage();

            return path == _explorerOptions.Value.RootDirectory
                ? CreateImageFromApplicationIcon()
                : path == _explorerOptions.Value.DeeperDirectoryName
                    ? CreateImageFromUndoIcon()
                    : ExtractIcon(path);
        }
        catch (Exception e)
        {
            _logger.LogError(CannotExtractIconDueToExceptionLogMessage, e.Message);
            return new BitmapImage();
        }
      
    }
    
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    private ImageSource CreateImageFromUndoIcon()
    {
        return CreateImageFromPath(SzUndoPath);
    }
    private ImageSource CreateImageFromApplicationIcon()
    {
        return CreateImageFromPath(SzIconPath);
    }

    private ImageSource CreateImageFromPath(string path)
    {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(path, UriKind.Absolute);
        bitmapImage.EndInit();
        bitmapImage.Freeze();
        return bitmapImage;
    }

    private ImageSource ExtractIcon(string path)
    {
        using var iconInfo = _extractor.Extract(path);

        if (iconInfo.HIcon == 0)
            return new BitmapImage();
        
        var bitmapSource = Imaging.CreateBitmapSourceFromHIcon(
            iconInfo.HIcon,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
        
        bitmapSource.Freeze();
        return bitmapSource;
    }
    
}