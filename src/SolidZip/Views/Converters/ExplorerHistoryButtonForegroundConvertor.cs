namespace SolidZip.Views.Converters;

[ValueConversion(typeof(bool), typeof(SolidColorBrush))]
public sealed class ExplorerHistoryButtonForegroundConvertor : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool showDefaultColor)
            return null;

        return showDefaultColor
            ? Application.Current.Resources["ForegroundColorBrush"]
            : Application.Current.Resources["ForegroundHoverColorBrush"];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}