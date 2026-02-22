namespace SolidZip.Views.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class VisibilityForHiddenDirectoryConverter(IUserJsonManager jsonManager) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return jsonManager.GetShowHiddenDirectoriesAsync().Result
            ? Visibility.Visible
            : value is true
                ?  Visibility.Collapsed
                :  Visibility.Visible;

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}