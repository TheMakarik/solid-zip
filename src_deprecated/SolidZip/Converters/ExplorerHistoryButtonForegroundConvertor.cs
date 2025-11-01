namespace SolidZip.Converters;

[ValueConversion(typeof(bool), typeof(SolidColorBrush))]
public sealed class ExplorerHistoryButtonForegroundConvertor : OneWayConvertor
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool showDefaultColor)
            return null;

        return showDefaultColor
            ? Application.Current.Resources["SzForegroundColorBrush"]
            : Application.Current.Resources["SzForegroundHoverColorBrush"];
    }
    
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}