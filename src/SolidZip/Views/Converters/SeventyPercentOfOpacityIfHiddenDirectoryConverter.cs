namespace SolidZip.Views.Converters;

[ValueConversion(typeof(bool), typeof(double))]
public class SeventyPercentOfOpacityIfHiddenDirectoryConverter(IUserJsonManager jsonManager) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return jsonManager.GetShowHiddenDirectoriesAsync().Result
            ? value is true 
                ? 0.7
                : 1.0 
            : 1.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}