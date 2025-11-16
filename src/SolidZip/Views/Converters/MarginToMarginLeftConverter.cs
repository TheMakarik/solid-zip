namespace SolidZip.Views.Converters;

[ValueConversion(typeof(Thickness), typeof(Thickness))]
public sealed class MarginToMarginLeftConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Thickness margin)
            return null;

        return new Thickness(margin.Left, 0 , 0 ,0);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}