using SolidZip.Core.Contracts.StateMachines;

namespace SolidZip.Views.Converters;

[ValueConversion(typeof(string), typeof(ImageSource))]
public sealed class PathToImageSourceConvertor(
    IExplorerStateMachine explorer, 
    PathsCollection paths,
    IOptions<ExplorerOptions> explorerOptions, 
    ILogger<PathToImageSourceConvertor>  logger ) : IValueConverter
{
    private readonly string SzIconPath = "pack://application:,,," + paths.IconPath;
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            if (value is not string path)
                return new BitmapImage();

            return path == explorerOptions.Value.RootDirectory
                ? CreateImageFromApplicationIcon()
                : ExtractIcon(path);
        }
        catch (Exception e)
        {
            logger.LogError("Cannot extract icon due to exception: {exception}", e.Message);
            return new BitmapImage();
        }
      
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
        using var iconInfo = explorer.GetIcon(path);

        if (iconInfo.HIcon == 0)
            return new BitmapImage();
        
        var bitmapSource = Imaging.CreateBitmapSourceFromHIcon(
            iconInfo.HIcon,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
        
        bitmapSource.Freeze();
        return bitmapSource;
    }


    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}