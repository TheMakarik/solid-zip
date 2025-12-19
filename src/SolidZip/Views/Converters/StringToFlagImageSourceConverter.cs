namespace SolidZip.Views.Converters;

[ValueConversion(typeof(string), typeof(ImageSource))]
public sealed class StringToFlagImageSourceConverter(
    IOptions<LocalizationOptions> localizationOptions, 
    PathsCollection paths,
    ILogger<StringToFlagImageSourceConverter> logger) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string supportedCulturesKey)
            return null;

        if (localizationOptions.Value.SupportedCultures.TryGetValue(supportedCulturesKey, out var cultureInfo))
            return GetLanguageIcon(cultureInfo);
        
        logger.LogError("Languege {lang} is not supported", supportedCulturesKey);
        return null;

    }

    private object? GetLanguageIcon(CultureInfo cultureInfo)
    {
        var iconName = cultureInfo.Name + ".png";
        var iconPath = Path.Combine(paths.LanguageIcons, iconName);

        if (!Directory.Exists(paths.LanguageIcons))
        {
            logger.LogError("Folder path {iconPath} not found", paths.LanguageIcons);
            return null;
        }
            
        
        if(File.Exists(iconPath))
            return CreateImageFromPath(iconPath);
        
        logger.LogError("Icon {icon} not found", iconName);
        return CreateImageFromPath(paths.UnknownLanguageIcon);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
    
    private ImageSource CreateImageFromPath(string path)
    {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(path, UriKind.Relative);
        bitmapImage.EndInit();
        bitmapImage.Freeze();
        return bitmapImage;
    }
}